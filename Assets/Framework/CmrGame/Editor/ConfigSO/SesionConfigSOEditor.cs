using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmrGame.Editor
{
    /// <summary>
    /// SessionConfigSO 的自定义 Inspector
    /// 仿照 ProcedureComponentInspector 的风格实现
    /// </summary>
    [CustomEditor(typeof(SessionConfigSO), true)]
    public class SessionConfigSOEditor : UnityEditor.Editor
    {
        // 序列化属性
        private SerializedProperty m_AvailablePhasesProp;
        private SerializedProperty m_DefaultPhaseProp;

        // 所有可用的 Phase 类型名称（全名）
        private string[] m_AllPhaseTypeNames;

        // 当前已选择的 Phase 列表（用于编辑）
        private List<string> m_CurrentAvailablePhases;

        // 入口 Phase 索引
        private int m_StartPhaseIndex = -1;

        // 折叠状态
        private bool m_PhaseListFoldout = true;

        protected virtual void OnEnable()
        {
            m_AvailablePhasesProp = serializedObject.FindProperty("AvailablePhases");
            m_DefaultPhaseProp = serializedObject.FindProperty("DefaultPhase");

            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. 绘制脚本引用
            ConfigSOEditor.DrawScriptField((ScriptableObject)target);

            EditorGUILayout.Space(5);

            // 2. 绘制基类字段（Description 等）
            DrawBaseClassFields();

            EditorGUILayout.Space(5);

            // 3. 绘制 Phase 选择区域（仿 ProcedureComponentInspector）
            DrawPhaseSelection();

            EditorGUILayout.Space(5);

            // 4. 绘制子类特有字段
            DrawChildClassFields();

            serializedObject.ApplyModifiedProperties();

            // 5. 绘制代码生成按钮
            ConfigSOEditor.DrawCodeGeneration(target as ConfigSOBase);
        }

        /// <summary>
        /// 绘制基类字段（ConfigSOBase 和 SessionConfigSO 中需要默认显示的字段）
        /// </summary>
        protected virtual void DrawBaseClassFields()
        {
            // 绘制 Description
            var descProp = serializedObject.FindProperty("Description");
            if (descProp != null)
            {
                EditorGUILayout.PropertyField(descProp);
            }
        }

        /// <summary>
        /// 绘制 Phase 选择区域（仿 ProcedureComponentInspector）
        /// </summary>
        protected virtual void DrawPhaseSelection()
        {
            // 验证入口 Phase
            if (string.IsNullOrEmpty(m_DefaultPhaseProp.stringValue))
            {
                EditorGUILayout.HelpBox("Start Phase is invalid.", MessageType.Error);
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                // 可折叠的 Phase 列表
                m_PhaseListFoldout = EditorGUILayout.Foldout(m_PhaseListFoldout, "Available Phases", true, EditorStyles.foldoutHeader);

                if (m_PhaseListFoldout)
                {
                    EditorGUI.indentLevel++;

                    if (m_AllPhaseTypeNames == null || m_AllPhaseTypeNames.Length == 0)
                    {
                        EditorGUILayout.HelpBox("No GamePhase types found in the project.", MessageType.Warning);
                    }
                    else
                    {
                        // 使用 ToggleLeft 绘制每个 Phase 选项
                        foreach (string phaseTypeName in m_AllPhaseTypeNames)
                        {
                            bool selected = m_CurrentAvailablePhases.Contains(phaseTypeName);
                            string displayName = GetDisplayName(phaseTypeName);

                            bool newSelected = EditorGUILayout.ToggleLeft(displayName, selected);

                            if (newSelected != selected)
                            {
                                if (newSelected)
                                {
                                    // 添加
                                    m_CurrentAvailablePhases.Add(phaseTypeName);
                                    WriteAvailablePhases();
                                }
                                else if (phaseTypeName != m_DefaultPhaseProp.stringValue)
                                {
                                    // 移除（但不能移除当前的入口 Phase）
                                    m_CurrentAvailablePhases.Remove(phaseTypeName);
                                    WriteAvailablePhases();
                                }
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // 入口 Phase 选择
                if (m_CurrentAvailablePhases.Count == 0)
                {
                    EditorGUILayout.HelpBox("Select available phases first.", MessageType.Info);
                }
                else
                {
                    // 构建显示名称数组
                    string[] displayNames = m_CurrentAvailablePhases
                        .Select(GetDisplayName)
                        .ToArray();

                    int newIndex = EditorGUILayout.Popup("Start Phase", m_StartPhaseIndex, displayNames);

                    if (newIndex != m_StartPhaseIndex && newIndex >= 0 && newIndex < m_CurrentAvailablePhases.Count)
                    {
                        m_StartPhaseIndex = newIndex;
                        m_DefaultPhaseProp.stringValue = m_CurrentAvailablePhases[newIndex];
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// 绘制子类特有的字段（自动遍历，无需手动维护排除列表）
        /// </summary>
        protected virtual void DrawChildClassFields()
        {
            // 获取除了已处理字段外的其他属性
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            bool hasChildFields = false;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // 跳过已处理的字段
                if (IsExcludedProperty(iterator.name))
                    continue;

                if (!hasChildFields)
                {
                    EditorGUILayout.LabelField("Specific Settings", EditorStyles.boldLabel);
                    hasChildFields = true;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }
        }

        /// <summary>
        /// 判断属性是否应被排除（基类字段）
        /// </summary>
        protected virtual bool IsExcludedProperty(string propertyName)
        {
            return propertyName == "m_Script" ||
                   propertyName == "Description" ||
                   propertyName == "AvailablePhases" ||
                   propertyName == "DefaultPhase";
        }

        /// <summary>
        /// 刷新类型名称
        /// </summary>
        private void RefreshTypeNames()
        {
            // 获取所有 GamePhase 子类
            var types = TypeCache.GetTypesDerivedFrom<GamePhase>()
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.FullName)
                .ToArray();

            m_AllPhaseTypeNames = types.Select(t => t.FullName).ToArray();

            // 读取当前已选择的 Phase
            ReadAvailablePhases();

            // 过滤掉已经不存在的类型
            int oldCount = m_CurrentAvailablePhases.Count;
            m_CurrentAvailablePhases = m_CurrentAvailablePhases
                .Where(x => m_AllPhaseTypeNames.Contains(x))
                .ToList();

            if (m_CurrentAvailablePhases.Count != oldCount)
            {
                WriteAvailablePhases();
            }
            else if (!string.IsNullOrEmpty(m_DefaultPhaseProp.stringValue))
            {
                RefreshStartPhaseIndex();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 从序列化属性读取已选择的 Phase
        /// </summary>
        private void ReadAvailablePhases()
        {
            m_CurrentAvailablePhases = new List<string>();
            int count = m_AvailablePhasesProp.arraySize;
            for (int i = 0; i < count; i++)
            {
                m_CurrentAvailablePhases.Add(m_AvailablePhasesProp.GetArrayElementAtIndex(i).stringValue);
            }
        }

        /// <summary>
        /// 写入已选择的 Phase 到序列化属性
        /// </summary>
        private void WriteAvailablePhases()
        {
            m_AvailablePhasesProp.ClearArray();

            if (m_CurrentAvailablePhases == null)
                return;

            m_CurrentAvailablePhases.Sort();

            for (int i = 0; i < m_CurrentAvailablePhases.Count; i++)
            {
                m_AvailablePhasesProp.InsertArrayElementAtIndex(i);
                m_AvailablePhasesProp.GetArrayElementAtIndex(i).stringValue = m_CurrentAvailablePhases[i];
            }

            RefreshStartPhaseIndex();
        }

        /// <summary>
        /// 刷新入口 Phase 索引
        /// </summary>
        private void RefreshStartPhaseIndex()
        {
            if (string.IsNullOrEmpty(m_DefaultPhaseProp.stringValue))
                return;

            m_StartPhaseIndex = m_CurrentAvailablePhases.IndexOf(m_DefaultPhaseProp.stringValue);

            if (m_StartPhaseIndex < 0)
            {
                // 入口 Phase 已被移除，清空
                m_DefaultPhaseProp.stringValue = null;
            }
        }

        /// <summary>
        /// 获取类型的显示名称（只显示类名）
        /// </summary>
        private string GetDisplayName(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return "(None)";

            int lastDot = fullTypeName.LastIndexOf('.');
            return lastDot >= 0 ? fullTypeName.Substring(lastDot + 1) : fullTypeName;
        }
    }
}