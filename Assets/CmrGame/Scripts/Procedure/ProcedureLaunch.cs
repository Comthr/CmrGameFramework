using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;
namespace CmrGame
{
    public class ProcedureLaunch:ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //CYJTodo:设置语言

            //CYJTodo:设置声音
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //切换至Splash流程
            ChangeState<ProcedureSplash>(procedureOwner);
        }
    }
}
