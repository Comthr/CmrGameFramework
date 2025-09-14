namespace CmrGame
{
    using CmrUnityGameFramework.Runtime;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public class UIFormViewGeneratorWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private string prefabDirectory = "Assets/UIForms"; // 指定扫描的目录
        private string outputDirectory = "Assets/Scripts/Generated/UIFormViews";
        private List<GameObject> prefabList = new List<GameObject>();

        [MenuItem("Tools/UIFormView Generator")]
        private static void OpenWindow()
        {
            UIFormViewGeneratorWindow window = GetWindow<UIFormViewGeneratorWindow>();
            window.titleContent = new GUIContent("UIFormView Generator");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("UIFormView Generator Tool", EditorStyles.boldLabel);

            // Prefab目录
            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefab Folder:", GUILayout.Width(100));
            prefabDirectory = EditorGUILayout.TextField(prefabDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Prefab Folder", prefabDirectory, "");
                if (!string.IsNullOrEmpty(path))
                {
                    prefabDirectory = path;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();

            // 输出目录
            GUILayout.BeginHorizontal();
            GUILayout.Label("Output Folder:", GUILayout.Width(100));
            outputDirectory = EditorGUILayout.TextField(outputDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Folder", outputDirectory, "");
                if (!string.IsNullOrEmpty(path))
                {
                    outputDirectory = path;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 刷新按钮
            if (GUILayout.Button("刷新 Prefab 列表"))
            {
                RefreshPrefabList();
            }

            GUILayout.Space(10);

            // 显示 Prefab 列表
            if (prefabList.Count > 0)
            {
                GUILayout.Label("Found Prefabs with UIFormLogic:");
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
                foreach (var prefab in prefabList)
                {
                    GUILayout.Label(prefab.name);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No Prefabs with UIFormLogic found in the selected folder.");
            }

            GUILayout.Space(10);

            // 生成按钮
            if (GUILayout.Button("生成 UIFormView"))
            {
                GenerateAllUIFormViews();
            }
        }

        /// <summary>
        /// 扫描指定目录下所有 Prefab，并筛选出挂了 UIFormLogic 的 Prefab
        /// </summary>
        private void RefreshPrefabList()
        {
            prefabList.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabDirectory });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<UIFormLogic>() != null)
                {
                    prefabList.Add(prefab);
                }
            }

            Debug.Log($"Found {prefabList.Count} prefabs with UIFormLogic.");
        }

        /// <summary>
        /// 生成所有 Prefab 对应的 UIFormView
        /// </summary>
        private void GenerateAllUIFormViews()
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach (var prefab in prefabList)
            {
                GenerateUIFormView(prefab);
            }

            AssetDatabase.Refresh();
            Debug.Log("UIFormView 生成完成！");
        }

        /// <summary>
        /// 根据 Prefab 生成对应 UIFormView
        /// </summary>
        private void GenerateUIFormView(GameObject prefab)
        {
            string logicName = prefab.GetComponent<UIFormLogic>().GetType().Name;
            string viewClassName = logicName.Replace("Logic", "View");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"public class {viewClassName}");
            sb.AppendLine("{");

            // 扫描 UIElementHandler
            var uiElements = prefab.GetComponentsInChildren<UIElementHandler>(true);
            foreach (var elem in uiElements)
            {
                string typeName = elem.GetType().Name; // UIButtonHandler / UILabelHandler
                string varName = elem.name.Replace(" ", "_");
                sb.AppendLine($"    public {typeName} {varName};");
            }

            sb.AppendLine();
            sb.AppendLine($"    public {viewClassName}(GameObject root)");
            sb.AppendLine("    {");

            foreach (var elem in uiElements)
            {
                string varName = elem.name.Replace(" ", "_");
                sb.AppendLine($"        {varName} = root.transform.Find(\"{GetHierarchyPath(elem.transform, prefab.transform)}\").GetComponent<{elem.GetType().Name}>();");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            string path = Path.Combine(outputDirectory, viewClassName + ".cs");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 获取从 Prefab 根节点到目标节点的层级路径
        /// </summary>
        private string GetHierarchyPath(Transform target, Transform root)
        {
            if (target == root)
                return "";
            return GetHierarchyPath(target.parent, root) + "/" + target.name;
        }
    }


}
