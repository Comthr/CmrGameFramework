using UnityEngine;
using UnityEditor;

namespace CmrGame
{
    public class UIFormViewGeneratorWindow : EditorWindow
    {
        private GameObject prefab;
        private string namespaceName = "CmrGame";
        private string className = "";
        private string outputPath = "Assets/CmrGame/Scripts/UI/Generated";
        private bool addSuffix = false;

        [MenuItem("CmrGame/Generate UIForm View")]
        public static void ShowWindow()
        {
            GetWindow<UIFormViewGeneratorWindow>("Generate UIForm View");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("单个 Prefab 生成 UIForm View 脚本工具", EditorStyles.boldLabel);
            // 拖拽 Prefab（必须含 UIFormLogic）
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab (含 UIFormLogic)", prefab, typeof(GameObject), false);
            // 输出路径输入
            outputPath = EditorGUILayout.TextField("输出路径", outputPath);
            // 命名空间输入
            namespaceName = EditorGUILayout.TextField("命名空间", namespaceName);
            // 类名输入（默认留空）
            className = EditorGUILayout.TextField("类名", className);
            // 后缀选项
            addSuffix = EditorGUILayout.Toggle("添加 _Generated 后缀", addSuffix);

            if (GUILayout.Button("生成"))
            {
                if (prefab == null)
                {
                    Debug.LogError("请先拖拽一个包含 UIFormLogic 的 Prefab。");
                }
                else
                {
                    // 调用静态生成方法
                    UIFormViewGenerator.GenerateViewScript(prefab, outputPath, className, namespaceName, addSuffix);
                }
            }
        }
    }
}
