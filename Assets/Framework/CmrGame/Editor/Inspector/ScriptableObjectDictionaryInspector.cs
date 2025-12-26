using UnityEditor;
using UnityEngine;
namespace CmrGame
{

    [CustomPropertyDrawer(typeof(ScriptableObjectDictionary<E_Scene,string>))]
    public class ScriptableObjectDictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entriesProp = property.FindPropertyRelative("entries");
            EditorGUI.PropertyField(position, entriesProp, label, true); // 直接用 Unity 内置 List 的绘制
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var entriesProp = property.FindPropertyRelative("entries");
            return EditorGUI.GetPropertyHeight(entriesProp, label, true);
        }
    }
}
