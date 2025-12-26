using CmrGameFramework.Procedure;
using CmrUnityFramework.Runtime;
using ProcedureOwner = CmrGameFramework.Fsm.IFsm<CmrGameFramework.Procedure.IProcedureManager>;
namespace CmrGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private GameSession _currentSession;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 1. 创建属于大厅的会话
            _currentSession = new LobbySession();

            _currentSession.Startup();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {                                                                                                              
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            _currentSession?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            if (_currentSession != null)
            {
                _currentSession.Shutdown();
                _currentSession = null;
            }
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}