using UnityEngine;
namespace CmrUnityFramework.Runtime
{
    /// <summary> 游戏框架组件抽象类 </summary>
    public class GameFrameworkComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            /*
             * CYJStep0-1:
             * 每个组件类都会在开始时将自身注册进GameEntry，就是在逻辑层面的游戏入口。
             * GameEntry中维护了一个游戏组件的链表（对这个链表做了封装），用来管理所有组件
             * 现在点进RegisterComponent的具体实现一探虚实吧→
             */
            GameEntry.RegisterComponent(this);
        }
    }
}
