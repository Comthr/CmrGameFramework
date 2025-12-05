using System;
using System.Linq;
using UnityEngine;

namespace CmrGame
{
    public partial class GlobalSettings
    {
        [Serializable]
        public class AssetSetting
        {
            [SerializeField]private string dataTablePath = "Assets/None/DataTable";
            [SerializeField]private string scenePath = "Assets/None/Scene";
            [SerializeField]private string uiFormAsset = "Assets/None/UI/UIForms";

            /// <summary>
            /// 获得表资源的完整路径，
            /// </summary>
            /// <param name="assetName"></param>
            /// <param name="extension"></param>
            /// <returns></returns>
            public string GetDataTableAsset(string namespaceName,string assetName, string extension)
            {
                return $"{dataTablePath}/{extension}/{namespaceName}_{assetName}.{extension}";
            }
            public string GetSceneAsset(string assetName)
            {
                return $"{scenePath}/{assetName}.unity";
            }
            public string GetUIFormAsset(string assetName)
            {
                return $"{uiFormAsset}/{assetName}.prefab";
            }
        }
    }
}
