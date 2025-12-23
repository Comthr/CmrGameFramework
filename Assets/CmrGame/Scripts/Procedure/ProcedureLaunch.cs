using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;
using CmrGameFramework.Resource;
using CmrUnityFramework.Runtime;
using UnityEngine;
namespace CmrGame
{
    public class ProcedureLaunch:ProcedureBase
    {
        private bool m_InitResourcesComplete = false;
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);



            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器模式，无需初始化资源就可以进入
                Log.Info("Editor resource mode detected.");
                m_InitResourcesComplete = true;
            }
            else if (GameEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                Log.Info("Package resource mode detected.");
                // 注意：使用单机模式并初始化资源前，需要先构建 AssetBundle 并复制到 StreamingAssets 中，否则会产生 HTTP 404 错误
                GameEntry.Resource.InitResources(OnInitResourcesComplete);
            }
            else
            {
                // 可更新模式
                Log.Info("Updatable resource mode detected.");
                Log.Error("可更新模式暂未实现");
                //CYJTodo:更新模式
            }

        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            // 初始化资源未完成则继续等待
            if (!m_InitResourcesComplete)
                return;

            ChangeState<ProcedurePreload>(procedureOwner);

        }
        private void OnInitResourcesComplete()
        {
            m_InitResourcesComplete = true;
            Log.Info("Init resources complete.");
        }
    }
}
