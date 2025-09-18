using CmrUnityGameFramework.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CmrGame
{
    public class UIFormViewBatchGeneratorWindow : EditorWindow
    {
        private string prefabScanPath = "Assets/CmrGame/UI";
        private string namespaceName = "CmrGame";
        private string outputPath = "Assets/CmrGame/Scripts/UI/Generated";
        private bool addSuffix = false;

        private List<GameObject> foundPrefabs = new List<GameObject>();
        private List<bool> prefabToggles = new List<bool>();

        [MenuItem("CmrGame/Batch Generate UIForm Views")]
        public static void ShowWindow()
        {
            GetWindow<UIFormViewBatchGeneratorWindow>("Batch Generate Views");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("批量 Prefab 生成 UIForm View 脚本工具", EditorStyles.boldLabel);
            prefabScanPath = EditorGUILayout.TextField("Prefab扫描路径", prefabScanPath);
            namespaceName = EditorGUILayout.TextField("命名空间 (可空)", namespaceName);
            outputPath = EditorGUILayout.TextField("输出路径", outputPath);
            addSuffix = EditorGUILayout.Toggle("添加 _Generated 后缀", addSuffix);

            EditorGUILayout.Space();
            if (GUILayout.Button("刷新"))
            {
                RefreshPrefabList();
            }

            // 列出包含 UIFormLogic 的 Prefab
            if (foundPrefabs.Count > 0)
            {
                EditorGUILayout.LabelField("包含 UIFormLogic 的 Prefab：", EditorStyles.label);
                for (int i = 0; i < foundPrefabs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    prefabToggles[i] = EditorGUILayout.Toggle(prefabToggles[i], GUILayout.Width(16));
                    EditorGUILayout.LabelField(foundPrefabs[i].name);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("生成"))
            {
                for (int i = 0; i < foundPrefabs.Count; i++)
                {
                    if (prefabToggles[i])
                    {
                        // 类名留空，使用静态方法内部默认逻辑名+View
                        UIFormViewGenerator.GenerateViewScript(foundPrefabs[i], outputPath, "", namespaceName, addSuffix);
                    }
                }
            }
        }

        private void RefreshPrefabList()
        {
            foundPrefabs.Clear();
            prefabToggles.Clear();
            // 在指定路径下查找所有 Prefab 资源
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabScanPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponentInChildren<UIFormLogic>(true) != null)
                {
                    foundPrefabs.Add(prefab);
                    prefabToggles.Add(true);
                }
            }
        }
    }
}
