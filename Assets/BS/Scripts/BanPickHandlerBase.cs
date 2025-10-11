
using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;

namespace BSTool
{
    public abstract class BanPickHandlerBase:IBanPickHandler
    {
        private bool m_IsHost;
        private bool m_IsBlue;
        protected IFsm<IBanPickHandler> m_Fsm;
        protected IFsmManager m_FsmManager;

        public bool IsHost => m_IsHost;
        public bool IsBlue => m_IsBlue;
        public string GetCharacterNameById(int id)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Initialize(IFsmManager fsmManager,bool isHost)
        {
            m_IsHost = isHost;
            m_FsmManager = fsmManager;
            m_Fsm = m_FsmManager.CreateFsm(this, GetStates());
        }
        public abstract void OnStart();
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        private BanPickStateBase[] GetStates()
        {
            return null;
        }
    }

}
