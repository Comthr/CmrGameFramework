using CmrGameFramework.Procedure;
using CmrUnityGameFramework.Runtime;
using ProcedureOwner = CmrGameFramework.Fsm.IFsm<CmrGameFramework.Procedure.IProcedureManager>;
namespace CmrGame
{
    public static class ProcedureExtension
    {
        public static void SetSceneData(this ProcedureBase procedure, ProcedureOwner owner, EScene scene)
        {
            CmrGameFramework.DataTable.IDataTable<DRScene> x = GameEntry.DataTable.GetDataTable<DRScene>();
            int nextSceneId = GameEntry.DataTable.GetDataTable<DRScene>().GetDataRow((int)scene).Id;
            owner.SetData<VarInt32>("nextSceneId", nextSceneId);
        }
    }

}
