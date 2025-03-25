using System.Collections.Generic;
using System.Linq;
using ComponentsNodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace KineticCore.Scripts.NodeEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(RendererNode))]
    public class RendererNodeEditor : XNodeEditor.NodeEditor
    {
        private KineticGraph _graph;
        
        private RendererNode _node;

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            _node = (RendererNode)target;
            _graph = _node.graph as KineticGraph;

            KineticSceneGraph sceneGraph = KineticGraphManager.GetKineticSceneGraph(_graph);

            if (sceneGraph == null)
            {
                Debug.LogWarning("Scene graph is null.");
                return;
            }

            // Получаем список всех Renderer в графе
            List<Renderer> renderers = sceneGraph.GetRefComponents<Renderer>();

            if (renderers.Count == 0)
            {
                Debug.LogWarning("No renderers found in the scene graph.");
            }

            // Создаём список для dropdown меню
            string[] rendererNames = renderers
                .Select((r, index) => 
                {
                    int count = renderers.Take(index).Count(x => x.gameObject.name == r.gameObject.name);
                    return count > 0 ? $"{r.gameObject.name}_{count + 1}" : r.gameObject.name;
                })
                .ToArray();

            if (rendererNames.Length > 0)
            {
                // Получаем индекс выбранного элемента, устанавливаем по умолчанию первый
                int selectedIndex = renderers.FindIndex(r => r.GetInstanceID() == _node._componentId);
                selectedIndex = Mathf.Max(selectedIndex, 0);  // По умолчанию выбираем первый элемент

                // Отображаем выпадающий список и обновляем индекс
                int newSelectedIndex = EditorGUILayout.Popup("Renderer", selectedIndex, rendererNames);

                // Если индекс изменился, обновляем компонент
                if (newSelectedIndex != selectedIndex)
                {
                    selectedIndex = newSelectedIndex;
                    _node.SetComponent(renderers[selectedIndex]);  // Обновляем компонент с новым выбранным значением
                    serializedObject.ApplyModifiedProperties();
                    NodeEditorWindow.current.Repaint();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Renderer nodes available in the graph.", MessageType.Warning);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
