using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;
using CmrGameFramework.Resource;
using CmrUnityGameFramework.Runtime;
using UnityEngine;
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

            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器模式，无需初始化资源就可以进入
                Log.Info("Editor resource mode detected.");
                ChangeState<ProcedurePreload>(procedureOwner);
            }
            else if (GameEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                Log.Info("Package resource mode detected.");
                ChangeState<ProcedureInitResources>(procedureOwner);
            }
            else
            {
                // 可更新模式
                Log.Info("Updatable resource mode detected.");
                Log.Error("可更新模式暂未实现");
                //CYJTodo:更新模式
            }
        }
    }
}
