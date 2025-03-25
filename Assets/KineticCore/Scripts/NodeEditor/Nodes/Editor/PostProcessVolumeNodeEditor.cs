using System;
using OutputNodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;
using XNode;

[CustomNodeEditor(typeof(PostProcessVolumeNode))]
public class PostProcessVolumeNodeEditor : NodeEditor
{
    private PostProcessVolumeNode node;
    private SerializedObject serializedNode;
    private SerializedProperty inputPortsProperty;

   public override void OnBodyGUI()
{
    node = (PostProcessVolumeNode)target;
    serializedNode = new SerializedObject(node);
    inputPortsProperty = serializedNode.FindProperty("inputPorts");
    serializedNode.Update();
    EditorGUILayout.PropertyField(serializedNode.FindProperty("Volume"), new GUIContent("Volume Profile"));
    serializedNode.ApplyModifiedProperties();
    VolumeProfile profile = node.Volume;
    if (profile == null)
    {
        EditorGUILayout.HelpBox("Choose Volume Profile to edit ports.", MessageType.Warning);
        return;
    }
    EditorGUILayout.Space();

    EditorGUILayout.LabelField("Input Ports", EditorStyles.boldLabel);

    // Draw dynamic ports list
    for (int i = node.properties.Count - 1; i >= 0; i--) // Перебираем с конца
    {
        var port = node.properties[i];

        EditorGUILayout.BeginVertical("box", GUILayout.Width(260));

        // Dropdown to choose effect
        List<string> effectOptions = GetAvailableEffects(profile);
        int selectedEffectIndex = effectOptions.IndexOf(port.EffectName);
        selectedEffectIndex = EditorGUILayout.Popup("Effect", selectedEffectIndex, effectOptions.ToArray());
        if (selectedEffectIndex >= 0)
        {
            port.EffectName = effectOptions[selectedEffectIndex];
        }

        // Dropdown to choose parameter
        List<string> parameterOptions = GetAvailableParameters(profile, port.EffectName);
        int selectedParamIndex = parameterOptions.IndexOf(port.ParameterName);
        selectedParamIndex = EditorGUILayout.Popup("Parameter", selectedParamIndex, parameterOptions.ToArray());
        if (selectedParamIndex >= 0)
        {
            port.ParameterName = parameterOptions[selectedParamIndex];
        }

        // Drawing port
        string portName = $"portValues[{i}]";
        NodePort inputPort = node.GetInputPort(portName);
        NodeEditorGUILayout.PortField(new GUIContent("Input Value"), inputPort, GUILayout.MinWidth(0));

        // Remove button (small 'X' button in top-right corner)
        Rect removeButtonRect = GUILayoutUtility.GetLastRect();
        removeButtonRect.x = removeButtonRect.xMax + 5;
        removeButtonRect.y = removeButtonRect.yMin - 15; 
        removeButtonRect.width = 15;
        removeButtonRect.height = 15;

        GUIStyle removeButtonStyle = new GUIStyle(GUI.skin.button);
        removeButtonStyle.normal.textColor = Color.white;
        removeButtonStyle.alignment = TextAnchor.MiddleCenter;
        removeButtonStyle.padding = new RectOffset(0, 0, 0, 0);

        if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
        {
            int index = i;
            string deletingPortName = portName;
            EditorApplication.delayCall += () =>
            { 
                    node.properties.RemoveAt(index);
                    // Удаление порта безопасно
                    node.RemoveDynamicPort(deletingPortName);
                    // Пересоздание массива node.portValues с новым размером
                    Array.Resize(ref node.portValues, node.properties.Count);

                    // Обновление портов после удаления
                    node.UpdatePorts();

                    // Переход к обновлению GUI
                    serializedNode.ApplyModifiedProperties();
                    NodeEditorWindow.current.Repaint();
            };
        }

        EditorGUILayout.EndVertical();
    }

    if (GUILayout.Button("Add Port"))
    {
        node.properties.Add(new PostProcessVolumePropertyData());
        node.AddDynamicInput(typeof(float), XNode.Node.ConnectionType.Override, XNode.Node.TypeConstraint.None, $"portValues[{node.properties.Count - 1}]");
        Array.Resize(ref node.portValues, node.properties.Count);
        node.portValues[node.properties.Count - 1] = 0f;
        node.UpdatePorts();
        serializedNode.ApplyModifiedProperties();
        NodeEditorWindow.current.Repaint();
    }

    serializedNode.ApplyModifiedProperties();
}

    private List<string> GetAvailableEffects(VolumeProfile profile)
    {
        List<string> effects = new List<string>();
        foreach (var component in profile.components)
        {
            effects.Add(component.GetType().Name);
        }
        return effects;
    }

    private List<string> GetAvailableParameters(VolumeProfile profile, string effectName)
    {
        if (string.IsNullOrEmpty(effectName)) return new List<string>();

        var effect = profile.components.Find(c => c.GetType().Name == effectName);
        if (effect == null) return new List<string>();

        List<string> parameters = new List<string>();
        var fields = effect.GetType().GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(VolumeParameter)))
            {
                parameters.Add(field.Name);
            }
        }
        return parameters;
    }
}
