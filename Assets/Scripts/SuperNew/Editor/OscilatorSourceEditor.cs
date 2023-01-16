using System.Linq;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[CustomEditor(typeof(OscilatorSource))]
[CanEditMultipleObjects]
public class OscilatorSourceEditor : Editor
{
    public override bool RequiresConstantRepaint() => Application.isPlaying;
    
    private OscilatorSource source;
    private SerializedProperty curve;
    private SerializedProperty extra;
    private SerializedProperty multiplyer;
    
    void OnEnable()
    {
        source = target as OscilatorSource;
        curve = serializedObject.FindProperty("curve");
        multiplyer = serializedObject.FindProperty("multiplyer");
        extra = serializedObject.FindProperty("extraValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(curve);
        EditorGUILayout.PropertyField(multiplyer);
        EditorGUILayout.PropertyField(extra);
        serializedObject.ApplyModifiedProperties();
        
        
        if (Application.isPlaying)
        {
            DrawCurve(source.Time, source.Value, source.MultipliedValue);
        }
    }

    private void DrawCurve(float sourceTime, float sourceValue, float sourceMultipliedValue)
    {
        EditorGUILayout.Space();

        // Graph area
        var rect = GUILayoutUtility.GetRect(128, 64);
        // Background
            Handles.DrawSolidRectangleWithOutline
                (rect, new Color(0.1f, 0.1f, 0.1f, 1), Color.clear);

        // Don't draw the actual graph if it isn't a repaint event.
        if (Event.current.type != EventType.Repaint) return;

        float curveWidthPerBeat = 128f / source.Curve.keys[source.Curve.keys.Length - 1].time;
        
        // Spectrum curve construction
            for (var i = 0; i < source.Curve.keys.Length; i++)
            {
                Keyframe key = source.Curve.keys[i];
               // Handles.DrawBezier(rect.xMin+key.time*curveWidthPerBeat, key.);
              
            }

            
            // Curve
            Handles.color = Color.green;
            float pos = source.Time * curveWidthPerBeat*3.3f;

            Handles.DrawLine(new Vector2(rect.xMin+pos, rect.yMin), new Vector2(rect.xMin+pos, rect.yMax));
            Handles.color = Color.white;
            Handles.DrawLine(new Vector2(rect.xMin,  rect.yMax - rect.height*sourceValue), new Vector2(rect.xMax, rect.yMax -rect.height*sourceValue));
            EditorGUI.LabelField(rect, sourceMultipliedValue.ToString("0.00"), new GUIStyle(){alignment = TextAnchor.MiddleCenter, fontSize = 35, normal = new GUIStyleState(){textColor = Color.white*0.5f}});
        }
}