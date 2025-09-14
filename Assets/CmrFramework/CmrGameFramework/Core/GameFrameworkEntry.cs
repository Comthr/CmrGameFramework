using System;
using System.Collections.Generic;
namespace CmrGameFramework
{    /// <summary> 游戏框架入口。 </summary>
    public class GameFrameworkEntry
    {
        private static readonly CachedLinkList<GameFrameworkModule> s_GameFrameworkModules = new CachedLinkList<GameFrameworkModule>();

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// CYJ:由BaseComponent中的Update调用
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (GameFrameworkModule module in s_GameFrameworkModules)
                module.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<GameFrameworkModule> current = s_GameFrameworkModules.Last; current != null; current = current.Previous)
                current.Value.Shutdown();

            s_GameFrameworkModules.Clear();
            ReferencePool.ClearAll();
            Utility.Marshal.FreeCachedHGlobal();
            GameFrameworkLog.SetLogHelper(null);
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            /*
             * CYJStep0-5-1:
             * 这里对泛型类型做了获取，一般来说拿到的肯定是以I开头的接口类型，
             * 这里很明显有一个隐性的规则就是你传入的泛型类型一定得是一个接口类型，并且去掉I后一定是一个继承自GameFrameworkModuel的类，不然就是不合法的命名或设计
             * 因为这里算是模块的内部实现，所以算是框架的使用规则。虽然这看起来十分牵强，但确实也不算太糟糕，
             * 不然如何通过接口访问到对应的模块类呢？
             * 本地化？那也得资源模块加载进来才行吧。（别问什么IO或Resource.Load，作为框架这些操作也应该在对应的模块去封装再去做）
             * 不过我似乎有一个好的办法。在某个.NET版本开始，就支持接口中的静态字段了，声明一个接口类的静态只读string来存对应的模型类名，这样就可以了吧？
             * 但是这对.NET的版本有要求，并且如果你对类型重命名的时候，忘记去修改string也会导致问题，所以也不算完美的解决。
             * 我有意去改成我的想法规则，依赖一些对.NET版本的判断来做一下分类处理，但仔细一想做这种事情也算是吃力不讨好。
             * 言归正传！！！！
             * 这边在判断合法后，就会去获取对应模块。类内有一个链表存储已经获取过的模块对象，如果没有获取过，就去创建一个。
             * 所以这个方法不仅可以访问模块类，也可以用来创建模块
             * 我们移步到CreateModule看看具体实现→
             */
            //CYJIdea:可能可以用接口中只读的静态字段来获取对应模块
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
                throw Utility.Exception.ThrowException(Utility.Text.Format("You must get module by interface, but '{0}' is not.", interfaceType.FullName));

            if (!interfaceType.FullName.StartsWith("CmrGameFramework.", StringComparison.Ordinal))
                throw Utility.Exception.ThrowException(Utility.Text.Format("You must get a CmrGameFramework module, but '{0}' is not.", interfaceType.FullName));

            string moduleName = Utility.Text.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not find CmrGameFramework module type '{0}'.", moduleName));

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        private static GameFrameworkModule GetModule(Type moduleType)
        {
            //CYJIdea:给链表加个哈希表实现？虽然模块的量级太小太小，做哈希表也是空间换时间，意义不大。
            foreach (GameFrameworkModule module in s_GameFrameworkModules)
                if (module.GetType() == moduleType)
                    return module;

            return CreateModule(moduleType);
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        private static GameFrameworkModule CreateModule(Type moduleType)
        {
            /*
             * CYJStep0-5-2:
             * 上面已经通过接口类型名获得了对应的模块类型名。
             * 这里就通过反射获取到对应类型的构造函数，创建一个模块对象
             * 然后就做一个根据优先级排序的插入，确保Priority更大的模块在链表靠前的位置
             * 这样就完成了对模块类的创建，这下我们可以安心的回到ConfigComponent做接下来的事情了→
             */
            GameFrameworkModule module = (GameFrameworkModule)Activator.CreateInstance(moduleType);
            if (module == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not create module '{0}'.", moduleType.FullName));

            LinkedListNode<GameFrameworkModule> current = s_GameFrameworkModules.First;

            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                    break;

                current = current.Next;
            }

            if (current != null)
                s_GameFrameworkModules.AddBefore(current, module);
            else
                s_GameFrameworkModules.AddLast(module);

            return module;
        }
    }
}
