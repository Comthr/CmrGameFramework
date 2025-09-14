using CmrGameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CmrUnityGameFramework.Runtime
{
    /// <summary> 游戏入口 </summary>
    public static class GameEntry
    {
        private static readonly CachedLinkList<GameFrameworkComponent> s_GameFrameworkComponents = new CachedLinkList<GameFrameworkComponent>();

        /// <summary> 游戏框架所在的场景编号 </summary>
        internal const int GameFrameworkSceneId = 0;

        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型</typeparam>
        /// <returns>要获取的游戏框架组件</returns>
        public static T GetComponent<T>() where T : GameFrameworkComponent
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型</param>
        /// <returns>要获取的游戏框架组件</returns>
        public static GameFrameworkComponent GetComponent(Type type)
        {
            LinkedListNode<GameFrameworkComponent> current = s_GameFrameworkComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                    return current.Value;

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <param name="typeName">要获取的游戏框架组件类型名称</param>
        /// <returns>要获取的游戏框架组件</returns>
        public static GameFrameworkComponent GetComponent(string typeName)
        {
            LinkedListNode<GameFrameworkComponent> current = s_GameFrameworkComponents.First;
            while (current != null)
            {
                Type type = current.Value.GetType();
                if (type.FullName == typeName || type.Name == typeName)
                    return current.Value;

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 关闭游戏框架
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架方式</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Log.Info("Shutdown Game Framework ({0})...", shutdownType);
            BaseComponent baseComponent = GetComponent<BaseComponent>();
            if (baseComponent != null)
            {
                baseComponent.Shutdown();
                baseComponent = null;
            }

            s_GameFrameworkComponents.Clear();

            if (shutdownType == ShutdownType.None)
                return;

            if (shutdownType == ShutdownType.Restart)
            {
                SceneManager.LoadScene(GameFrameworkSceneId);
                return;
            }

            if (shutdownType == ShutdownType.Quit)
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return;
            }
        }

        /// <summary>
        /// 注册游戏框架组件
        /// </summary>
        /// <param name="gameFrameworkComponent">要注册的游戏框架组件</param>
        internal static void RegisterComponent(GameFrameworkComponent gameFrameworkComponent)
        {
            /*
             * CYJStep0-2:
             * 直接来个省流。所有组件类都会调用注册方法，被加入链表中，并保证了组件在链表中的唯一性
             * 但这个链表对普通链表做了封装，新增了一个缓存功能，实际使用和普通链表没区别
             * 目前你已经知道组件是如何被注册的了，这时候可以回到BaseComponent的Awake方法，我们去看接下来的逻辑→
             */
            if (gameFrameworkComponent == null)
            {
                Log.Error("Game Framework component is invalid.");
                return;
            }

            Type type = gameFrameworkComponent.GetType();

            LinkedListNode<GameFrameworkComponent> current = s_GameFrameworkComponents.First;
            //CYJ:查重，确保组件唯一性
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Log.Error("Game Framework component type '{0}' is already exist.", type.FullName);
                    return;
                }

                current = current.Next;
            }

            s_GameFrameworkComponents.AddLast(gameFrameworkComponent);
        }
    }
}
