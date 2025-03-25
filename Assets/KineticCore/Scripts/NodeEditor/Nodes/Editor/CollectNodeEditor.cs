using KineticNodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

    [CustomNodeEditor(typeof(CollectNode))]
    public class CollectNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            
            CollectNode collectNode = target as CollectNode;
            if (collectNode == null) return;

            float outputValue = collectNode.outputValue;
            string outputString = outputValue.ToString();
            
            int charCount = outputString.Length;
            int fontSize = Mathf.Clamp(90 / charCount, 10, 50); // Динамический размер шрифта
            
            GUIStyle boldLargeStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField(outputString, boldLargeStyle, GUILayout.Height(fontSize * 2));
        }
    }