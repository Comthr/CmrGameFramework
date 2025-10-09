
using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;

namespace BSTool
{
    public abstract class BanPickHandlerBase:IBanPickHandler
    {
        private int m_State;
        IFsm<IBanPickHandler> m_Fsm;
        IFsmManager m_FsmManager;

        public void Initialize(IFsmManager fsmManager)
        {
            m_FsmManager = fsmManager;
            m_Fsm = m_FsmManager.CreateFsm(this, GetStates());
        }
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        private BanPickStateBase[] GetStates()
        {
            return null;
        }
    }

}
