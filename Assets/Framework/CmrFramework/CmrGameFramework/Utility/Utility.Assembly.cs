using System;
using System.Collections.Generic;
namespace CmrGameFramework
{
    public static partial class Utility
    {
        /// <summary>
        /// 程序集相关的实用函数
        /// </summary>
        public static class Assembly
        {
            private static readonly System.Reflection.Assembly[] s_Assemblies;

            private static readonly Dictionary<string, Type> s_CachedTypes;
            static Assembly()
            {
                s_Assemblies = null;
                s_CachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);
                s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 获取已加载的程序集中的所有类型。
            /// </summary>
            /// <returns>已加载的程序集中的所有类型。</returns>
            public static Type[] GetTypes()
            {
                List<Type> results = new List<Type>();
                foreach (System.Reflection.Assembly assembly in s_Assemblies)
                {
                    results.AddRange(assembly.GetTypes());
                }

                return results.ToArray();
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static Type GetType(string typeName)
            {
                if (string.IsNullOrEmpty(typeName))
                    throw Exception.ThrowException("Type name is invalid.");

                if (s_CachedTypes.TryGetValue(typeName, out Type type))
                    return type;

                type = Type.GetType(typeName);
                if (type != null)
                {
                    s_CachedTypes.Add(typeName, type);
                    return type;
                }

                foreach (System.Reflection.Assembly assembly in s_Assemblies)
                {
                    type = Type.GetType(Text.Format("{0}, {1}", typeName, assembly.FullName));
                    if (type != null)
                    {
                        s_CachedTypes.Add(typeName, type);
                        return type;
                    }
                }

                return null;
            }
        }
    }
}
