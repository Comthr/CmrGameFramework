
using System;

namespace CmrGameFramework.Fsm
{
    /// <summary>
    /// 有限状态机状态基类
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型</typeparam>
    public abstract class FsmState<T> where T : class
    {
        public FsmState() { }
        /// <summary>
        /// 初始化有限状态机状态时调用。
        /// CYJ:在Fsm.Create中被调用，意味着在Fsm被初始化创建时就会调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnInit(IFsm<T> fsm) { }

        /// <summary>
        /// 进入有限状态机状态时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnEnter(IFsm<T> fsm) { }

        /// <summary>
        /// 轮询有限状态机状态时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float elapseSeconds, float realElapseSeconds) { }

        /// <summary>
        /// 离开有限状态机状态时调用
        /// CYJ:当状态机被清理时，isShutdown为true
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发</param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown) { }

        /// <summary>
        /// 销毁有限状态机状态时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnDestroy(IFsm<T> fsm) { }

        /// <summary>
        /// 切换当前有限状态机状态
        /// </summary>
        /// <typeparam name="TState"> 要切换到的有限状态机状态类型 </typeparam>
        /// <param name="fsm"> 有限状态机引用 </param>
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>
        {
            (((Fsm<T>)fsm) ?? throw Utility.Exception.ThrowException("FSM is invalid.")).ChangeState<TState>();
        }
        /// <summary>
        /// 切换当前有限状态机状态
        /// </summary>
        /// <param name="fsm"> 有限状态机引用 </param>
        /// <param name="stateType"> 要切换到的有限状态机状态类型 </param>
        protected void ChangeState(IFsm<T> fsm, Type stateType)
        {
            Fsm<T> obj = ((Fsm<T>)fsm) ?? throw Utility.Exception.ThrowException("FSM is invalid.");
            if ((object)stateType == null)
            {
                throw Utility.Exception.ThrowException("State type is invalid.");
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                throw Utility.Exception.ThrowException(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            obj.ChangeState(stateType);
        }

    }
}

