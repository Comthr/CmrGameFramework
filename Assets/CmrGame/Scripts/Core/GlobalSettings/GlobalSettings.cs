using UnityEngine;

namespace CmrGame
{
    [CreateAssetMenu(fileName = "GlobalSettings", menuName = "CmrGame/Global Settings")]
    public partial class GlobalSettings:ScriptableObject
    {
        [Header("Global Settings")]

        [Header("Asset Settings")]
        [SerializeField]private AssetSetting asset;
        public AssetSetting Asset => asset;
    }
}
