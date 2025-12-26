using CmrUnityFramework.Runtime;

namespace CmrGame
{
    public static partial class Utility
    {
        public static class Asset
        {        
            // 资源根路径模板
            private const string SESSION_RES_ROOTPATH = "Assets/Game/{0}/Res";
            /// <summary>
            /// 获取资源根目录
            /// </summary>
            private static string GetResRoot(E_GameSession game)
            {
                return string.Format(SESSION_RES_ROOTPATH, game.ToString());
            }

            public static string GetConfigAsset(E_GameSession game, string assetRelativePath)
            {
                return $"{GetResRoot(game)}/Config/{assetRelativePath}";
            }

            public static string GetDataTableAsset(E_GameSession game, string tableName)
            {
                return $"{GetResRoot(game)}/DataTable/{tableName}";
            }

            public static string GetUIFormAsset(E_GameSession game, string formName)
            {
                return $"{GetResRoot(game)}/UI/Forms/{formName}";
            }

            public static string GetEntityAsset(E_GameSession game, string group, string entityName)
            {
                return $"{GetResRoot(game)}/Entity/{group}/{entityName}";
            }

            public static string GetMusicAsset(E_GameSession game, string musicName)
            {
                return $"{GetResRoot(game)}/Audio/Music/{musicName}";
            }
            public static string GetSoundAsset(E_GameSession game, string soundName)
            {
                return $"{GetResRoot(game)}/Audio/Sound/{soundName}";
            }

            public static string GetSceneAsset(E_GameSession game, string sceneName)
            {
                return $"{GetResRoot(game)}/Scene/{sceneName}";
            }
        }
    }
}