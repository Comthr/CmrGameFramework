using UnityEngine;
using UnityEditor;

namespace CmrGame
{
    public class UIFormViewGeneratorWindow : EditorWindow
    {
        private GameObject prefab;
        private string namespaceName = "None";
        private string className = "";
        private string outputPath = "Assets/None/Scripts/UI/Generated";
        private bool addSuffix = false;

        [MenuItem("None/Generate UIForm View")]
        public static void ShowWindow()
        {
            GetWindow<UIFormViewGeneratorWindow>("Generate UIForm View");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("单个 Prefab 生成 UIForm View 脚本工具", EditorStyles.boldLabel);
            //Prefab
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab (含 UIFormLogic)", prefab, typeof(GameObject), false);
            // 输出路径输入
            outputPath = EditorGUILayout.TextField("输出路径", outputPath);
            // 命名空间输入
            namespaceName = EditorGUILayout.TextField("命名空间", namespaceName);
            // 类名输入
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
                    UIFormViewGenerator.GenerateViewScript(prefab, outputPath, className, namespaceName, addSuffix);
                }
            }
        }
    }
}
