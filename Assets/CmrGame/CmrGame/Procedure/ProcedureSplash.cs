using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;
using CmrGameFramework.Resource;

using CmrUnityGameFramework.Runtime;

namespace CmrGame
{
    public class ProcedureSplash:ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            // CYJTodo: 这里可以播放一个 Splash 动画

        }
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            //CYJTodo:在这里可以做版本更新或直接进入游戏的判断
            if (GFEntry.Base.EditorResourceMode)
            {
                // 编辑器模式
                Log.Info("Editor resource mode detected.");
                ChangeState<ProcedurePreload>(procedureOwner);
            }
            else if (GFEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                Log.Info("Package resource mode detected.");
                ChangeState<ProcedureInitResources>(procedureOwner);
            }
            else
            {
                // 可更新模式
                Log.Info("Updatable resource mode detected.");

                //CYJTodo:更新模式
            }

        }
    }
}
