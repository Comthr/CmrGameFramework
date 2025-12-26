using UnityEngine;
using UnityEditor;

namespace CmrGame.Editor
{
    [CustomEditor(typeof(ConfigSOBase), true)]
    public class ConfigSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认属性
            DrawDefaultInspector();

            // 绘制代码生成部分
            DrawCodeGeneration(target as ConfigSOBase);
        }

        /// <summary>
        /// 绘制代码生成按钮（静态方法，可被任何 Editor 调用）
        /// </summary>
        public static void DrawCodeGeneration(ConfigSOBase configSO)
        {
            if (configSO == null)
                return;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Code Generation", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Accessor Code", GUILayout.Height(30)))
            {
                ConfigCodeGenerator.Generate(configSO);
            }
        }
        public static void DrawScriptField(ScriptableObject target)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(target), typeof(ScriptableObject), false);
            EditorGUI.EndDisabledGroup();
        }
    }
}