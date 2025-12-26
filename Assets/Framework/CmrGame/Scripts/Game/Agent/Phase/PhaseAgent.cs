using CmrGameFramework.Fsm;
using CmrUnityFramework.Runtime;

namespace CmrGame
{
    /// <summary>
    /// 阶段代理
    /// </summary>
    public class PhaseAgent : GameAgent
    {
        private IFsm<PhaseAgent> m_PhaseFsm;

        /// <summary>
        /// 获取当前正在运行的阶段
        /// </summary>
        public GamePhase CurrentPhase
        {
            get
            {
                if (m_PhaseFsm == null)
                    throw CmrGameFramework.Utility.Exception.ThrowException("You must initialize PhaseAgent first.");

                return (GamePhase)m_PhaseFsm.CurrentState;
            }
        }
        /// <summary>
        /// 获取当前阶段持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (m_PhaseFsm == null)
                    throw CmrGameFramework.Utility.Exception.ThrowException("You must initialize PhaseAgent first.");

                return m_PhaseFsm.CurrentStateTime;
            }
        }


        public override void Shutdown()
        {
            if (m_PhaseFsm != null)
            {
                GameEntry.Fsm.DestroyFsm(m_PhaseFsm);
                m_PhaseFsm = null;
            }
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="phases">所有相关的阶段实例</param>
        public override void Initialize(GameSession session)
        {
            if (m_PhaseFsm != null)
            {
                Log.Warning("阶段代理不能被重复初始化，因此本次调用会被弃置。");
                return;
            }

            //得到本命名空间内的所有阶段


            //m_PhaseFsm = GameEntry.Fsm.CreateFsm(this, phases);
        }

        /// <summary>
        /// 启动状态机 (类似 ProcedureManager.StartProcedure)
        /// </summary>
        /// <typeparam name="T">入口阶段</typeparam>
        public void StartPhase<T>() where T : GamePhase
        {
            if (m_PhaseFsm == null)
            {
                Log.Error("PhaseAgent has not been initialized yet.");
                return;
            }

            m_PhaseFsm.Start<T>();
        }

        /// <summary>
        /// 是否拥有某个阶段
        /// </summary>
        public bool HasPhase<T>() where T : GamePhase
        {
            if (m_PhaseFsm == null) 
                return false;
            return m_PhaseFsm.HasState<T>();
        }
    }
}