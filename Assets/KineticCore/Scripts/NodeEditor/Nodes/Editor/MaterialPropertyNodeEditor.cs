using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNodeEditor;
using OutputNodes;
using UnityEditor;
using XNode;

[CustomNodeEditor(typeof(MaterialPropertyNode))]
public class MaterialPropertyNodeEditor : NodeEditor
{
    private MaterialPropertyNode node;
    private SerializedObject serializedNode;
    private SerializedProperty inputPortsProperty;

    public override void OnBodyGUI()
    {
        node = (MaterialPropertyNode)target;
        serializedNode = new SerializedObject(node);
        inputPortsProperty = serializedNode.FindProperty("inputPorts");
        serializedNode.Update();

        // Добавляем порт для Renderer
        NodePort rendererPort = node.GetInputPort("renderer");
        NodeEditorGUILayout.PortField(new GUIContent("Renderer"), rendererPort);

        Renderer renderer = node.GetInputValue<Renderer>("renderer", node.renderer);

        if (renderer == null)
        {
            EditorGUILayout.HelpBox("Choose Renderer to edit ports.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Input Ports", EditorStyles.boldLabel);

        for (int i = node.properties.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(260));

            List<string> materialProperties = GetMaterialFloatProperties(renderer);
            int selectedPropertyIndex = materialProperties.IndexOf(node.properties[i]);
            selectedPropertyIndex = EditorGUILayout.Popup("Property", selectedPropertyIndex, materialProperties.ToArray());

            if (selectedPropertyIndex >= 0)
            {
                node.properties[i] = materialProperties[selectedPropertyIndex];
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

    private List<string> GetMaterialFloatProperties(Renderer renderer)
    {
        List<string> floatProperties = new List<string>();

        if (renderer == null || renderer.sharedMaterial == null)
            return floatProperties;

        Shader shader = renderer.sharedMaterial.shader;
        int propertyCount = ShaderUtil.GetPropertyCount(shader);

        for (int i = 0; i < propertyCount; i++)
        {
            // Проверка типа свойства на Float
            if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Float 
                || ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Range 
                || ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Int)
            {
                string propertyName = ShaderUtil.GetPropertyName(shader, i);

                // Проверка на наличие атрибута HideInInspector
                var property = shader.GetType().GetProperty(propertyName);
                var hideInInspectorAttribute = property?.GetCustomAttribute<HideInInspector>();

                if (hideInInspectorAttribute == null)
                {
                    floatProperties.Add(propertyName);
                }
            }
        }

        return floatProperties;
    }
}
