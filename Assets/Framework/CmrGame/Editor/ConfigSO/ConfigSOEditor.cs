using UnityEngine;
using UnityEditor;

namespace CmrGame.Editor
{
    [CustomEditor(typeof(ConfigSOBase), true)]
    public class ConfigSOEditor : UnityEditor.Editor
    {
        // 样式缓存
        private GUIStyle m_PathLabelStyle;

        public override void OnInspectorGUI()
        {
            ConfigSOBase so = target as ConfigSOBase;


            DrawScriptField(so);

            // 2. 默认属性 (包含了 BelongSession 枚举)
            DrawDefaultInspector();

            // 3. 绘制工具箱
            DrawConfigToolkit(so);
        }

        public static void DrawConfigToolkit(ConfigSOBase so)
        {
            if (so == null) return;

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Config Toolkit", EditorStyles.boldLabel);

            // --- 区域 A: 路径预览 (Info Box) ---
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // 预计算路径
            string codePath = ConfigToolkit.GetAccessorPath(so);
            string dataPath = ConfigToolkit.GetDataPath(so);

            EditorGUILayout.LabelField("Target Session:", so.BelongSession.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            // 使用 TextArea 显示路径，方便复制且支持换行
            DrawPathLabel("Code Path:", codePath);
            DrawPathLabel("Data Path:", dataPath);

            EditorGUILayout.EndVertical();

            // --- 区域 B: 操作按钮 ---
            EditorGUILayout.Space(5);

            // 第一排：生成代码 (最重要的步骤，给个大按钮)
            GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // 淡蓝色
            if (GUILayout.Button("1. Generate Accessor Code", GUILayout.Height(35)))
            {
                ConfigToolkit.GenerateAccessor(so);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(2);

            // 第二排：数据操作 (导出/导入 并排)
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(0.8f, 1f, 0.8f); // 淡绿色
            if (GUILayout.Button("2. Export (.txt)", GUILayout.Height(30)))
            {
                ConfigToolkit.ExportData(so);
            }

            GUI.backgroundColor = new Color(1f, 0.9f, 0.7f); // 淡黄色
            if (GUILayout.Button("3. Import (.txt)", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Import Config",
                    "Overwrite ScriptableObject values with .txt data?", "Yes", "Cancel"))
                {
                    ConfigToolkit.ImportData(so);
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawPathLabel(string title, string path)
        {
            EditorGUILayout.LabelField(title, EditorStyles.miniLabel);
            // 截断 Assets 前面的部分显示相对路径，或者显示全路径
            GUI.enabled = false; // 只读
            EditorGUILayout.TextField(path);
            GUI.enabled = true;
        }

        public static void DrawScriptField(ScriptableObject target)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(target), typeof(ScriptableObject), false);
            EditorGUI.EndDisabledGroup();
        }
    }
}