using CmrGameFramework.Procedure;
using CmrUnityFramework.Runtime;
using ProcedureOwner = CmrGameFramework.Fsm.IFsm<CmrGameFramework.Procedure.IProcedureManager>;
namespace CmrGame
{
    public static class ProcedureExtension
    {
        private const string nextSceneId = "id";
        public static void SetNextScene(this ProcedureBase procedure, ProcedureOwner owner, E_Scene scene)
        {
            owner.SetData<VarInt32>(nextSceneId, (int)scene);
        }

        public static string GetSceneName(this ProcedureBase procedure, ProcedureOwner owner)
        {
            int sceneId = owner.GetData<VarInt32>(nextSceneId);

            return ((E_Scene)sceneId).ToString();
        }
    }
}
