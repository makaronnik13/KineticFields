using UnityEditor;
using UnityEngine;
using XNodeEditor;
using static XNodeEditor.NodeEditor;

[CustomNodeEditor(typeof(InputSONode))]
public class InputSONodeEditor : NodeEditor
{
    private InputSONode node;

    public override void OnBodyGUI()
    {
        node = (InputSONode)target;

        GUILayout.BeginVertical();
        GUILayout.Space(10);

        if (Application.isPlaying)
        {
            if (node.inputSO != null)
            {
                DrawSignalValueBar(node.signalValue);
            }
        }
        else
        {
            var newInputSO = (InputSO)EditorGUILayout.ObjectField("Input SO", node.inputSO, typeof(InputSO), false);
            if (newInputSO != node.inputSO)
            {
                node.inputSO = newInputSO;
            }
        }

        GUILayout.Space(10);

        // Добавляем поле для выхода
        NodeEditorGUILayout.PortField(target.GetOutputPort("signalValue"));

        GUILayout.EndVertical();
    }


    public override void OnHeaderGUI()
    {
        node = (InputSONode)target;

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        if (node.inputSO != null)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            EditorGUILayout.LabelField(node.inputSO.name, headerStyle);
            GUILayout.EndVertical();
        }
    }

    public override void OnCreate()
    {
        base.OnCreate(); 
        EditorApplication.update += RepaintNode; 
    }

    
    
    private void DrawSignalValueBar(float signalValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        float barWidth = 180;

        float fillWidth = Mathf.Clamp(signalValue * barWidth, 0, barWidth);

        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, barWidth, 35), new Color(0.1f, 0.1f, 0.1f));
        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, fillWidth, 35), new Color(0.0f, 0.5f, 0.0f));

        GUILayout.EndHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        // Draw signal value label
        Rect valueRect = EditorGUILayout.GetControlRect(GUILayout.Height(15));
        EditorGUI.LabelField(valueRect, signalValue.ToString("F2"), new GUIStyle(EditorStyles.label) {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState { textColor = Color.white }
        });

        GUILayout.EndHorizontal();
    }

    private void RepaintNode()
    {
        if (node != null)
        {
            NodeEditorWindow.current.Repaint();
        }
    }
}
