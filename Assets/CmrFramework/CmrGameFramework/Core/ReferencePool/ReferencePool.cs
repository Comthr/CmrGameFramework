using System;
using System.Collections.Generic;

namespace CmrGameFramework
{
    /// <summary> 引用池 </summary>
    public partial class ReferencePool
    {
        private static readonly Dictionary<Type, ReferenceCollection> s_ReferenceCollections = new Dictionary<Type, ReferenceCollection>();

        private static bool m_EnableStrictCheck = false;

        /// <summary>
        /// 获取或设置是否开启强制检查
        /// CYJ: 开启后可以防止多次调用Release导致同一个对象重复入队
        /// </summary>
        public static bool EnableStrictCheck
        {
            get
            {
                return m_EnableStrictCheck;
            }
            set
            {
                m_EnableStrictCheck = value;
            }
        }

        /// <summary> 获取引用池的数量 </summary>
        public static int Count => s_ReferenceCollections.Count;

        /// <summary>
        /// 获取所有引用池的信息
        /// </summary>
        /// <returns> 所有引用池的信息 </returns>
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int num = 0;
            ReferencePoolInfo[] array = null;
            lock (s_ReferenceCollections)
            {
                array = new ReferencePoolInfo[s_ReferenceCollections.Count];
                foreach (KeyValuePair<Type, ReferenceCollection> s_ReferenceCollection in s_ReferenceCollections)
                {
                    array[num++] = new ReferencePoolInfo(s_ReferenceCollection.Key, s_ReferenceCollection.Value.UnusedReferenceCount, 
                        s_ReferenceCollection.Value.UsingReferenceCount, s_ReferenceCollection.Value.AcquireReferenceCount, 
                        s_ReferenceCollection.Value.ReleaseReferenceCount, s_ReferenceCollection.Value.AddReferenceCount, 
                        s_ReferenceCollection.Value.RemoveReferenceCount);
                }

                return array;
            }
        }

        /// <summary> 清除所有引用池 </summary>
        public static void ClearAll()
        {
            lock (s_ReferenceCollections)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> s_ReferenceCollection in s_ReferenceCollections)
                    s_ReferenceCollection.Value.RemoveAll();
                s_ReferenceCollections.Clear();
            }
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T"> 引用类型 </typeparam>
        /// <returns> 获取到的引用 </returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        /// <returns> 获取到的引用 </returns>
        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <param name="reference"> 引用 </param>
        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw Utility.Exception.ThrowException("Reference is invalid.");
            }

            Type type = reference.GetType();
            InternalCheckReferenceType(type);
            GetReferenceCollection(type).Release(reference);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <typeparam name="T"> 引用类型 </typeparam>
        /// <param name="count"> 追加数量 </param>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        /// <param name="count"> 追加数量 </param>
        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <typeparam name="T"> 引用类型 </typeparam>
        /// <param name="count"> 移除数量 </param>
        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        /// <param name="count"> 移除数量 </param>
        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <typeparam name="T"> 引用类型 </typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        /// <summary>
        /// CYJ：检查引用类型是否合法，
        /// 若不合法，抛出异常。
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        private static void InternalCheckReferenceType(Type referenceType)
        {
            if (m_EnableStrictCheck)
            {
                if ((object)referenceType == null)
                    throw Utility.Exception.ThrowException("Reference type is invalid.");

                if (!referenceType.IsClass || referenceType.IsAbstract)
                    throw Utility.Exception.ThrowException("Reference type is not a non-abstract class type.");

                if (!typeof(IReference).IsAssignableFrom(referenceType))
                    throw Utility.Exception.ThrowException(Utility.Text.Format("Reference type '{0}' is invalid.", referenceType.FullName));
            }
        }

        /// <summary>
        /// CYJ：获取到指定类型的引用池
        /// </summary>
        /// <param name="referenceType"> 引用类型 </param>
        /// <returns></returns>
        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if ((object)referenceType == null)
            {
                throw Utility.Exception.ThrowException("ReferenceType is invalid.");
            }

            ReferenceCollection value = null;
            lock (s_ReferenceCollections)
            {
                if (!s_ReferenceCollections.TryGetValue(referenceType, out value))
                {
                    value = new ReferenceCollection(referenceType);
                    s_ReferenceCollections.Add(referenceType, value);
                }
            }
            return value;
        }
    }
}
