using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public static partial class Constant
    {
        public static class Asset
        {
            private static string Namespace => Global.MainNamespace;
            private static string dataTablePath = "Assets/{0}/DataTable/csv";
            private static string scenePath = "Assets/{0}/Scene";
            private static string uiFormAsset = "Assets/{0}/Prefabs/UIForms";
            private static string extensionCSV = ".csv";
            private static string extensionScene = ".scene";
            private static string extensionPrefab = ".prefab";
            /// <summary>
            /// 获取游戏数据表资源路径。
            /// </summary>
            /// <param name="tableName">表名</param>
            /// <param name="gameType">游戏</param>
            /// <returns></returns>
            public static string GetGameDataTableAsset(string tableName,E_Game gameType = E_Game.None)
            {
                if (gameType == E_Game.None)
                    gameType = GameEntry.CurrentGame;
                string filePath = string.Format(dataTablePath, gameType.ToString());
                return $"{filePath}/{tableName}{extensionCSV}";
            }
            /// <summary>
            /// 获取全局数据表资源路径
            /// </summary>
            /// <param name="tableName">表名</param>
            /// <returns></returns>
            public static string GetGlobalDataTableAsset(string tableName)
            {
                string filePath = string.Format(dataTablePath, Namespace);
                return $"{filePath}/{tableName}{extensionCSV}";
            }
            public static string GetGameSceneAsset(string tableName, E_Game gameType = E_Game.None)
            {
                if (gameType == E_Game.None)
                    gameType = GameEntry.CurrentGame;
                string filePath = string.Format(scenePath, gameType.ToString());
                return $"{filePath}/{tableName}{extensionScene}";
            }
            public static string GetGlobalSceneAsset(string tableName)
            {
                string filePath = string.Format(scenePath, Namespace);
                return $"{filePath}/{tableName}{extensionScene}";
            }
            public static string GetGameUIFormAsset(string tableName, E_Game gameType = E_Game.None)
            {
                if (gameType == E_Game.None)
                    gameType = GameEntry.CurrentGame;
                string filePath = string.Format(uiFormAsset, gameType.ToString());
                return $"{filePath}/{tableName}{extensionPrefab}";
            }
            public static string GetGlobalUIFormAsset(string tableName)
            {
                string filePath = string.Format(uiFormAsset, Namespace);
                return $"{filePath}/{tableName}{extensionPrefab}";
            }
        }
    }

}
