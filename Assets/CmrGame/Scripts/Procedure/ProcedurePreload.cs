using CmrGameFramework;
using CmrGameFramework.Event;
using CmrGameFramework.Procedure;
using CmrUnityGameFramework.Runtime;
using System.Collections.Generic;
using System.Data;
using ProcedureOwner = CmrGameFramework.Fsm.IFsm<CmrGameFramework.Procedure.IProcedureManager>;

namespace CmrGame
{
    public class ProcedurePreload : ProcedureBase
    {
        public EventComponent Event => GFEntry.Event;
        private static readonly EDataRow[] preloadTables = new EDataRow[]
        {
            EDataRow.DRScene,
            EDataRow.DRUIForm
        };

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            m_LoadedFlag.Clear();

            PreloadResources();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            //确保所有加载完成后再离开
            foreach (KeyValuePair<string, bool> loadedFlag in m_LoadedFlag)
            {
                if (!loadedFlag.Value)
                    return;
            }
            CmrGameFramework.DataTable.IDataTable<DRScene> x = GFEntry.DataTable.GetDataTable<DRScene>();

            int nextSceneId = GFEntry.DataTable.GetDataTable<DRScene>().GetDataRow((int)EScene.Menu).Id;
            procedureOwner.SetData<VarInt32>("nextSceneId", nextSceneId);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

        private void PreloadResources()
        {
            // Preload configs
            //LoadConfig("DefaultConfig");

            // Preload data tables
            foreach (EDataRow dataTableName in preloadTables)
            {
                LoadDataTable(dataTableName);
            }

            // Preload dictionaries

            //LoadDictionary("Default");

            // Preload fonts

            //LoadFont("MainFont");
        }

        private void LoadConfig(string configName)
        {
            //CYJTodo:LoadConfig
            //string configAssetName = AssetUtility.GetConfigAsset(configName, false);
            //m_LoadedFlag.Add(configAssetName, false);
            //GFEntry.Config.ReadData(configAssetName, this);
        }

        private void LoadDataTable(EDataRow dataTable)
        {
            string dataTableName= dataTable.ToString();
            string namespaceName = typeof(EDataRow).Namespace;
            string dataTableAssetName = GFEntry.GlobalSettings.Asset.GetDataTableAsset(namespaceName, dataTableName, "csv");
            m_LoadedFlag.Add(dataTableAssetName, false);
            GFEntry.DataTable.LoadDataTable(namespaceName,dataTableName, dataTableAssetName, this);
        }

        private void LoadDictionary(string dictionaryName)
        {            
            //CYJTodo:LoadDictionary
            //string dictionaryAssetName = AssetUtility.GetDictionaryAsset(dictionaryName, false, GFEntry.Localization.Language);
            //m_LoadedFlag.Add(dictionaryAssetName, false);
            //GFEntry.Localization.ReadData(dictionaryAssetName, this);
        }

        private void LoadFont(string fontName)
        {
            //CYJTodo:LoadFont
            //m_LoadedFlag.Add(Utility.Text.Format("Font.{0}", fontName), false);
            //GFEntry.Resource.LoadAsset(AssetUtility.GetFontAsset(fontName), Constant.AssetPriority.FontAsset, new LoadAssetCallbacks(
            //    (assetName, asset, duration, userData) =>
            //    {
            //        m_LoadedFlag[Utility.Text.Format("Font.{0}", fontName)] = true;
            //        UGuiForm.SetMainFont((Font)asset);
            //        Log.Info("Load font '{0}' OK.", fontName);
            //    },

            //    (assetName, status, errorMessage, userData) =>
            //    {
            //        Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName, errorMessage);
            //    }));
        }

        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs)e;
            if (ne.UserData != this)
                return;

            m_LoadedFlag[ne.ConfigAssetName] = true;
            Log.Info("Load config '{0}' OK.", ne.ConfigAssetName);
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs)e;
            if (ne.UserData != this)
                return;

            Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigAssetName, ne.ConfigAssetName, ne.ErrorMessage);
        }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
                return;

            m_LoadedFlag[ne.DataTableAssetName] = true;
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            LoadDataTableFailureEventArgs ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this)
                return;

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName, ne.DataTableAssetName, ne.ErrorMessage);
        }

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
                return;

            m_LoadedFlag[ne.DictionaryAssetName] = true;
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
                return;

            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }
    }
}
