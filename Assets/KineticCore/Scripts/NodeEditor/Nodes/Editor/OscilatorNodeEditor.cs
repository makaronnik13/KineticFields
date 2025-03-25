using System;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using InputNode;

[CustomNodeEditor(typeof(OscillatorNode))]
public class OscillatorNodeEditor : NodeEditor
{
    private OscillatorNode oscillatorNode;
    
    public override void OnBodyGUI()
    {
        if (oscillatorNode == null) oscillatorNode = target as OscillatorNode;

        serializedObject.Update();

        // Draw the default node header
        NodeEditorGUILayout.PortField(oscillatorNode.GetOutputPort("outputValue"));

        // Custom curve field with beat division ticks
        var curveProperty = serializedObject.FindProperty("curve");
        var beatDivisionProperty = serializedObject.FindProperty("beatDivision");
        
        Rect curveRect = GUILayoutUtility.GetRect(200, 100);
        EditorGUI.DrawRect(curveRect, new Color(0.15f, 0.15f, 0.15f));

        // Draw curve
        
        // Extract and draw the curve properly
        AnimationCurve curve = curveProperty.animationCurveValue;
        curve = EditorGUI.CurveField(curveRect, curve);
        curveProperty.animationCurveValue = curve;
        
        // Получаем значение NormalizedTime и вычисляем его в пикселях относительно ширины кривой
        float normalizedX = Mathf.Lerp(curveRect.x, curveRect.xMax, oscillatorNode.NormalizedTime);

        //Рисуем вертикальную линию, которая будет отображать NormalizedTime
        Handles.color = Color.white;
        Handles.DrawLine(new Vector3(normalizedX, curveRect.y), new Vector3(normalizedX, curveRect.yMax));
        

// Вычисляем значение кривой в точке NormalizedTime
        float curveY = oscillatorNode.Curve.Evaluate(oscillatorNode.NormalizedTime);

// Переводим значение кривой в пиксели на высоте кривой (по оси Y)
        float normalizedY = Mathf.Lerp(curveRect.yMax, curveRect.yMin, curveY);

// Отрисовываем вертикальную линию
        Handles.color = Color.white;
        Handles.DrawLine(new Vector3(normalizedX, curveRect.y), new Vector3(normalizedX, curveRect.yMax));

// Отрисовываем кружок на пересечении вертикальной линии и кривой
        float circleRadius = 3f; // Радиус кружка
        Handles.DrawSolidDisc(new Vector3(normalizedX, normalizedY, 0), Vector3.forward, circleRadius);
        
        
        // Draw beat division ticks
        int division = (int)Mathf.Pow(2, beatDivisionProperty.enumValueIndex - 6);
        if (division < 1) division = 1;

        float step = curveRect.width / division;
        for (int i = 1; i < division; i++)
        {
            float x = curveRect.x + step * i;
            Handles.color = Color.gray;

            // Vertical tick line with adjusted height and width, drawn from the bottom
            float tickHeight = curveRect.height / 5; // 1/5 of the height
            float tickWidth = 3; // 3 times thicker
            Handles.DrawLine(new Vector3(x, curveRect.y + curveRect.height), new Vector3(x, curveRect.y + curveRect.height - tickHeight));
        }

        
        // Open curve editor on click
        if (Event.current.type == EventType.MouseDown && curveRect.Contains(Event.current.mousePosition))
        {
            CurveEditorWindow.ShowWindow(curveProperty);
        }

        // Draw PlayMode enum
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"), new GUIContent("Mode"));

        // Custom dropdown for BeatDivision with formatted labels
        string[] beatDivisionLabels = new string[]
        {
            "1|64", "1|32", "1|16", "1|8", "1|4", "1|2", "1", "2", "4", "8", "16", "32", "64"
        };

        beatDivisionProperty.enumValueIndex = EditorGUILayout.Popup("Beat Division", beatDivisionProperty.enumValueIndex, beatDivisionLabels);
        

        EditorGUILayout.PropertyField(serializedObject.FindProperty("random"), new GUIContent("Random"));
        
        // Show randomInterval and randomDistance only if mode is Random
        if (oscillatorNode.random)
        {
            string[] randomIntervalLabels = new string[]
            {
                "1|8", "1|4", "1|2", "1", "2", "4", "8"
            };
            
            // Get the index for the current random interval (mapping float to index)
            int selectedRandomIntervalIndex = Mathf.Clamp(Array.IndexOf(new float[] { 1f / 8f, 1f / 4f, 1f / 2f, 1f, 2f, 4f, 8f }, oscillatorNode.randomInterval), 0, randomIntervalLabels.Length - 1);

            // Show the popup with the interval options
            selectedRandomIntervalIndex = EditorGUILayout.Popup("Random Interval", selectedRandomIntervalIndex, randomIntervalLabels);

            // Map the selected index back to the correct float value
            oscillatorNode.randomInterval = new float[] { 1f / 8f, 1f / 4f, 1f / 2f, 1f, 2f, 4f, 8f }[selectedRandomIntervalIndex];


            // Check if beatDivision is less than 1 to hide Random Distance
            if (GetMaxRandomDistance(oscillatorNode.beatDivision)!=0)
            {
                oscillatorNode.randomDistance = EditorGUILayout.IntSlider("Random Distance", oscillatorNode.randomDistance, 0,  GetMaxRandomDistance(oscillatorNode.beatDivision));
            }
            else
            {
                oscillatorNode.randomDistance = 0;
            }
        }


        serializedObject.ApplyModifiedProperties();
    }

    public override int GetWidth() => 300;

    public override Color GetTint() => new Color(0.1f, 0.4f, 0.2f);
    
    private int GetMaxRandomDistance(OscillatorNode.BeatDivision beatDivision)
    {
        switch (beatDivision)
        {
            case OscillatorNode.BeatDivision.OneSixtyFourth: return 0;
            case OscillatorNode.BeatDivision.OneThirtySecond: return 0;
            case OscillatorNode.BeatDivision.OneSixteenth: return 0;
            case OscillatorNode.BeatDivision.OneEighth: return 0;
            case OscillatorNode.BeatDivision.OneFourth: return 0;
            case OscillatorNode.BeatDivision.OneHalf: return 0;
            case OscillatorNode.BeatDivision.One: return 1;
            case OscillatorNode.BeatDivision.Two: return 2;
            case OscillatorNode.BeatDivision.Four: return 4;
            case OscillatorNode.BeatDivision.Eight: return 8;
            case OscillatorNode.BeatDivision.Sixteen: return 16;
            case OscillatorNode.BeatDivision.ThirtyTwo: return 32;
            case OscillatorNode.BeatDivision.SixtyFour: return 64;
            default: return 0;
        }
    }
    
    public class CurveEditorWindow : EditorWindow
    {
        private SerializedProperty curveProperty;

        public static void ShowWindow(SerializedProperty curveProp)
        {
            CurveEditorWindow window = GetWindow<CurveEditorWindow>(true, "Edit Curve");
            window.curveProperty = curveProp;
            window.Show();
        }

        private void OnGUI()
        {
            if (curveProperty == null) Close();
            curveProperty.animationCurveValue = EditorGUILayout.CurveField("Edit Curve", curveProperty.animationCurveValue);
        }
    }
}