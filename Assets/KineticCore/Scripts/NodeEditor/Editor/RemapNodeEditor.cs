using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(RemapNode))]
public class RemapNodeEditor : NodeEditor
{
    private RemapNode node;

    public override void OnBodyGUI()
    {
        node = (RemapNode)target;
        serializedObject.Update();

        GUILayout.BeginVertical();
        GUILayout.Space(10);

        // Поле входного порта
        NodeEditorGUILayout.PortField(target.GetInputPort("inputValue"));

        GUILayout.Space(5);

        // Редактируемые параметры с помощью слайдеров
        node.inMin = EditorGUILayout.FloatField("Input Min", node.inMin);
        node.inMax = EditorGUILayout.FloatField("Input Max", node.inMax);
        node.outMin = EditorGUILayout.FloatField("Output Min", node.outMin);
        node.outMax = EditorGUILayout.FloatField("Output Max", node.outMax);

        GUILayout.Space(10);
        // Отображение входного значения слайдером
        float inputValue = node.GetInputValue("inputValue", 0f);
        // Отображение выходного значения слайдером
        float outputValue = node.GetValue(node.GetOutputPort("outputValue")) as float? ?? 0f;
        
        // Визуальная шкала
        if (Application.isPlaying)
        {
            DrawSignalValueBar(outputValue, inputValue);
        }

        GUILayout.Space(10);

        // Поле выходного порта
        NodeEditorGUILayout.PortField(target.GetOutputPort("outputValue"));

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSignalValueBar(float value, float targetValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        float barWidth = 250;
        float fillWidth = Mathf.Clamp((value - node.outMin) / (node.outMax - node.outMin) * barWidth, 0, barWidth);
        float targetPos = Mathf.Clamp((targetValue - node.inMin) / (node.inMax - node.inMin) * barWidth, 0, barWidth);

        // Фон
        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, barWidth, 35), new Color(0.1f, 0.1f, 0.1f));
        // Заполненная область
        EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 5, fillWidth, 35), new Color(0.0f, 0.5f, 0.0f));
        // Линия целевого значения
        EditorGUI.DrawRect(new Rect(lastRect.x + targetPos, lastRect.y + 5, 3, 35), new Color(0.8f, 0.8f, 0.7f));

        // Значение в центре
        Rect valueRect = EditorGUILayout.GetControlRect(GUILayout.Height(35));
        EditorGUI.LabelField(valueRect, value.ToString("F2"), new GUIStyle(EditorStyles.label)
        {
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
