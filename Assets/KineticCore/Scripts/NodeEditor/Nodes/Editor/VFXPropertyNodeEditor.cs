using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XNodeEditor;
using OutputNodes;
using UnityEditor;
using UnityEngine.VFX;
using XNode;

[CustomNodeEditor(typeof(VFXPropertyNode))]
public class VFXPropertyNodeEditor : NodeEditor
{
    private VFXPropertyNode node;
    private SerializedObject serializedNode;
    private SerializedProperty inputPortsProperty;

    public override void OnBodyGUI()
    {
        node = (VFXPropertyNode)target;
        serializedNode = new SerializedObject(node);
        inputPortsProperty = serializedNode.FindProperty("inputPorts");
        serializedNode.Update();

        // Добавляем порт для Renderer
        NodePort rendererPort = node.GetInputPort("visualEffect");
        NodeEditorGUILayout.PortField(new GUIContent("VFX"), rendererPort);

        VisualEffect visualEffect = node.GetInputValue<VisualEffect>("visualEffect");
        
        if (visualEffect == null)
        {
            EditorGUILayout.HelpBox("Choose vfx to edit ports.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Input Ports", EditorStyles.boldLabel);

        for (int i = node.properties.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(260));

            
            List<VFXExposedProperty> vfxProperites = GetVFXFloatProperties(visualEffect);
            List<string> propertiesNames = vfxProperites.Select(p => p.name).ToList();
            
            
            int selectedPropertyIndex = propertiesNames.IndexOf(node.properties[i]);
            selectedPropertyIndex = EditorGUILayout.Popup("Property", selectedPropertyIndex, propertiesNames .ToArray());

            if (selectedPropertyIndex >= 0)
            {
                node.properties[i] = propertiesNames[selectedPropertyIndex];
            }

            string portName = $"portValues[{i}]";
            NodePort inputPort = node.GetInputPort(portName);
            NodeEditorGUILayout.PortField(new GUIContent("Input Value"), inputPort, GUILayout.MinWidth(0));

            Rect removeButtonRect = GUILayoutUtility.GetLastRect();
            removeButtonRect.x = removeButtonRect.xMax + 5;
            removeButtonRect.y = removeButtonRect.yMin - 15;
            removeButtonRect.width = 15;
            removeButtonRect.height = 15;
            
            if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
            {
                int index = i;
                string deletingPortName = portName;
                EditorApplication.delayCall += () =>
                {
                    node.properties.RemoveAt(index);
                    node.RemoveDynamicPort(deletingPortName);
                    Array.Resize(ref node.portValues, node.properties.Count);
                    node.UpdatePorts();
                    serializedNode.ApplyModifiedProperties();
                    NodeEditorWindow.current.Repaint();
                };
            }
            
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Port"))
        {
            node.properties.Add("");
            node.AddDynamicInput(typeof(float), XNode.Node.ConnectionType.Override, XNode.Node.TypeConstraint.None, $"portValues[{node.properties.Count - 1}]");
            Array.Resize(ref node.portValues, node.properties.Count);
            node.portValues[node.properties.Count - 1] = 0f;
            node.UpdatePorts();
            serializedNode.ApplyModifiedProperties();
            NodeEditorWindow.current.Repaint();
        }

        serializedNode.ApplyModifiedProperties();
    }

    private List<VFXExposedProperty> GetVFXFloatProperties(VisualEffect visualEffect)
    {
        List<VFXExposedProperty> properties = new List<VFXExposedProperty>();
        if (visualEffect == null)
            return properties;
 
        visualEffect.visualEffectAsset.GetExposedProperties(properties);


        return properties.Where(p=> visualEffect.HasFloat(p.name) || visualEffect.HasBool(p.name) || visualEffect.HasInt(p.name)).ToList();
    }
}
