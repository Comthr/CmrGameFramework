using CmrGameFramework.Procedure;
using CmrUnityFramework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace CmrUnityFramework.Editor
{
    [CustomEditor(typeof(ProcedureComponent))]
    internal sealed class ProcedureComponentInspector : GameFrameworkInspector
    {
        /// <summary> CYJ:属性，选用的流程类型列表 </summary>
        private SerializedProperty m_AvailableProcedureTypeNames = null;

        /// <summary> CYJ:属性，入口流程类型 </summary>
        private SerializedProperty m_EntranceProcedureTypeName = null;

        /// <summary> CYJ:所有流程类型名 </summary>
        private string[] m_ProcedureTypeNames = null;

        /// <summary> CYJ:选用的流程类型，对应属性的中间变量 </summary>
        private List<string> m_CurrentAvailableProcedureTypeNames = null;

        private int m_EntranceProcedureIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ProcedureComponent t = (ProcedureComponent)target;

            if (string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
                EditorGUILayout.HelpBox("Entrance procedure is invalid.", MessageType.Error);
            else if (EditorApplication.isPlaying)
                EditorGUILayout.LabelField("Current Procedure", t.CurrentProcedure == null ? "None" : t.CurrentProcedure.GetType().ToString());

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                GUILayout.Label("Available Procedures", EditorStyles.boldLabel);
                if (m_ProcedureTypeNames.Length == 0)
                    EditorGUILayout.HelpBox("There is no available procedure.", MessageType.Warning);
                else
                {
                    //CYJ:选择流程
                    foreach (string procedureTypeName in m_ProcedureTypeNames)
                    {
                        bool selected = m_CurrentAvailableProcedureTypeNames.Contains(procedureTypeName);
                        if (selected != EditorGUILayout.ToggleLeft(procedureTypeName, selected))
                        {
                            if (!selected)
                            {
                                m_CurrentAvailableProcedureTypeNames.Add(procedureTypeName);
                                WriteAvailableProcedureTypeNames();
                            }
                            else if (procedureTypeName != m_EntranceProcedureTypeName.stringValue)
                            {
                                m_CurrentAvailableProcedureTypeNames.Remove(procedureTypeName);
                                WriteAvailableProcedureTypeNames();
                            }
                        }
                    }
                }

                if (m_CurrentAvailableProcedureTypeNames.Count== 0)
                    EditorGUILayout.HelpBox("Select available procedures first.", MessageType.Info);
                else
                {
                    EditorGUILayout.Separator();

                    //CYJ:选择入口流程
                    int selectedIndex = EditorGUILayout.Popup("Entrance Procedure", m_EntranceProcedureIndex, 
                        m_CurrentAvailableProcedureTypeNames.ToArray());

                    if (selectedIndex != m_EntranceProcedureIndex)
                    {
                        m_EntranceProcedureIndex = selectedIndex;
                        m_EntranceProcedureTypeName.stringValue = m_CurrentAvailableProcedureTypeNames[selectedIndex];
                    }
                }

            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_AvailableProcedureTypeNames = serializedObject.FindProperty("m_AvailableProcedureTypeNames");
            m_EntranceProcedureTypeName = serializedObject.FindProperty("m_EntranceProcedureTypeName");

            RefreshTypeNames();
        }

        /// <summary>
        /// CYJ:刷新流程名
        /// </summary>
        private void RefreshTypeNames()
        {
            //CYJ:先获取所有类型对应的流程类型名
            m_ProcedureTypeNames = Type.GetRuntimeTypeNames(typeof(ProcedureBase));
            //CYJ:将所有选用的流程写入可用列表
            ReadAvailableProcedureTypeNames();
            //CYJ:过滤掉已经不存在的流程
            int oldCount = m_CurrentAvailableProcedureTypeNames.Count;
            m_CurrentAvailableProcedureTypeNames = m_CurrentAvailableProcedureTypeNames.Where(x => m_ProcedureTypeNames.Contains(x)).ToList();

            if (m_CurrentAvailableProcedureTypeNames.Count != oldCount)
                WriteAvailableProcedureTypeNames();
            else if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
                WriteEntranceProcedureTypeName();
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// CYJ:从选用的流程属性获取到当前可用的流程列表
        /// </summary>
        private void ReadAvailableProcedureTypeNames()
        {
            m_CurrentAvailableProcedureTypeNames = new List<string>();
            int count = m_AvailableProcedureTypeNames.arraySize;
            for (int i = 0; i < count; i++)
                m_CurrentAvailableProcedureTypeNames.Add(m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue);
        }

        private void WriteAvailableProcedureTypeNames()
        {
            m_AvailableProcedureTypeNames.ClearArray();
            if (m_CurrentAvailableProcedureTypeNames == null)
                return;

            m_CurrentAvailableProcedureTypeNames.Sort();
            int count = m_CurrentAvailableProcedureTypeNames.Count;
            for (int i = 0; i < count; i++)
            {
                m_AvailableProcedureTypeNames.InsertArrayElementAtIndex(i);
                m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue = m_CurrentAvailableProcedureTypeNames[i];
            }
            if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
                WriteEntranceProcedureTypeName();
        }
        /// <summary>
        /// CYJAdd:写入入口类型名
        /// </summary>
        private void WriteEntranceProcedureTypeName()
        {
            //CYJ:找到入口流程所在下标，若小于0，则表示还没有入口
            if (string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
                return;
            m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
            if (m_EntranceProcedureIndex < 0)
            {
                m_EntranceProcedureTypeName.stringValue = null;
            }
        }
    }
}
