using System.Collections.Generic;
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
    private SerializedProperty extraValue;
    
    void OnEnable()
    {
        source = target as SignalSource;
        frequencyGap = serializedObject.FindProperty("gap");
        signalType = serializedObject.FindProperty("signalType");
        interpolate = serializedObject.FindProperty("interpolate");
        interpolationTime = serializedObject.FindProperty("interpolaionTime");
        multiplyer = serializedObject.FindProperty("multiplyer");
        extraValue = serializedObject.FindProperty("extraValue");
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
        EditorGUILayout.PropertyField(extraValue);
        
        serializedObject.ApplyModifiedProperties();
        
        
        if (Application.isPlaying)
        {
            IEnumerable<float> data = source.GetSpectrumData();
            if (data.Count()!= 0)
            {
                SpectrumDrawer.DrawGraph(data.Select(s=>s*source.multiplyer).ToArray(), source.V, source.MultipliedValue);
            }
        }
    }
}