using KineticNodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(RelaxationNode))]
public class RelaxationNodeEditor : NodeEditor
{
    private RelaxationNode node;

    public override void OnBodyGUI()
    {
        node = (RelaxationNode)target;

        GUILayout.BeginVertical();
        GUILayout.Space(10);

        // Редактируемые параметры с помощью слайдеров
        node.relaxationTime = EditorGUILayout.Slider("Gain", node.relaxationTime, 0.01f, 2.0f);
        node.gainTime = EditorGUILayout.Slider("Relax", node.gainTime, 0.05f, 5f);

        
        if (Application.isPlaying)
        {
            GUILayout.Space(15);

            DrawSignalValueBar(node.outputValue, node.inputValue);
            GUILayout.Space(10);
        }


        
        // Поле входного порта
        NodeEditorGUILayout.PortField(target.GetInputPort("inputValue"));
        
        // Поле выходного порта
        NodeEditorGUILayout.PortField(target.GetOutputPort("outputValue"));

        GUILayout.EndVertical();
    }

    private void DrawSignalValueBar(float value, float targetValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        float barWidth = 270;
        float fillWidth = Mathf.Clamp(value * barWidth, 0, barWidth);
        float targetPos = Mathf.Clamp(targetValue * barWidth, 0, barWidth);
        
        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, barWidth, 35), new Color(0.1f, 0.1f, 0.1f));
        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, fillWidth, 35), new Color(0.0f, 0.5f, 0.0f));
        EditorGUI.DrawRect(new Rect(lastRect.x + targetPos , lastRect.y + 5, 3, 35), new Color(0.8f, 0.8f, 0.7f));
 
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        // Draw signal value label
        Rect valueRect = EditorGUILayout.GetControlRect(GUILayout.Height(35));
        EditorGUI.LabelField(valueRect, value.ToString("F2"), new GUIStyle(EditorStyles.label) {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState { textColor = Color.white }
        });

        GUILayout.Space(15);
        GUILayout.EndHorizontal();
    }
    
    public override void OnCreate()
    {
        base.OnCreate(); 
        EditorApplication.update += RepaintNode; 
    }

    private void RepaintNode()
    {
        if (node != null)
        {
            NodeEditorWindow.current.Repaint();
        }
    }
}