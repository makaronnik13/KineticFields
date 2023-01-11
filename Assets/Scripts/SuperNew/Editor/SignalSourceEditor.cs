using System.Linq;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SignalSource))]
[CanEditMultipleObjects]
public class SignalSourceEditor : Editor
{
    public override bool RequiresConstantRepaint() => Application.isPlaying;
    
    private SignalSource source;
    private SerializedProperty frequencyGap;
    private SerializedProperty signalType;
    private SerializedProperty interpolate;
    private SerializedProperty interpolationTime;
    private SerializedProperty multiplyer;
    
    void OnEnable()
    {
        source = target as SignalSource;
        frequencyGap = serializedObject.FindProperty("gap");
        signalType = serializedObject.FindProperty("signalType");
        interpolate = serializedObject.FindProperty("interpolate");
        interpolationTime = serializedObject.FindProperty("interpolaionTime");
        multiplyer = serializedObject.FindProperty("multiplyer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(frequencyGap);
        EditorGUILayout.PropertyField(signalType);
        EditorGUILayout.PropertyField(interpolate);
        if (interpolate.boolValue)
        {
            EditorGUILayout.PropertyField(interpolationTime);
        }
        EditorGUILayout.PropertyField(multiplyer);
        
        serializedObject.ApplyModifiedProperties();
        
        
        if (Application.isPlaying)
        {
            float[] data = source.GetSpectrumData();
            if (data.Length != 0)
            {
                SpectrumDrawer.DrawGraph(data, source.V, source.MultipliedValue);
            }
        }
    }
}