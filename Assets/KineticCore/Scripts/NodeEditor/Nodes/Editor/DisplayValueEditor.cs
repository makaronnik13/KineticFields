using ToolsNodes;
using UnityEditor;
using UnityEngine;

namespace XNodeEditor.Examples {

    [CustomNodeEditor(typeof(DisplayValue))]
    public class DisplayValueEditor : NodeEditor {

        public override void OnBodyGUI() {
            base.OnBodyGUI();
            
            DisplayValue displayValueNode = target as DisplayValue;
            object obj = displayValueNode.GetValue();
            
            if (obj != null && float.TryParse(obj.ToString(), out float value)) {
                int charCount = value.ToString().Length;
                int fontSize = Mathf.Clamp(90 / charCount, 10, 50); // Динамический размер шрифта
                
                GUIStyle boldLargeStyle = new GUIStyle(EditorStyles.label) {
                    fontSize = fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                EditorGUILayout.LabelField(value.ToString(), boldLargeStyle, GUILayout.Height(fontSize * 2));
            }
        }
    }
}