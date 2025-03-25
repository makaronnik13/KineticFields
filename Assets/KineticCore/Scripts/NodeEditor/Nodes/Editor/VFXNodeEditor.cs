using System.Collections.Generic;
using System.Linq;
using ComponentsNodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using XNodeEditor;

namespace KineticCore.Scripts.NodeEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(VFXNode))]
    public class VFXNodeEditor : XNodeEditor.NodeEditor
    {
        private KineticGraph _graph;
        
        private VFXNode _node;

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            _node = (VFXNode)target;
            _graph = _node.graph as KineticGraph;

            KineticSceneGraph sceneGraph = KineticGraphManager.GetKineticSceneGraph(_graph);

            if (sceneGraph == null)
            {
                Debug.LogWarning("Scene graph is null.");
                return;
            }

            // Получаем список всех Renderer в графе
            List<VisualEffect> vfxs = sceneGraph.GetRefComponents<VisualEffect>();

            if (vfxs.Count == 0)
            {
                Debug.LogWarning("No vfx found in the scene graph.");
            }

            // Создаём список для dropdown меню
            string[] rendererNames = vfxs
                .Select((r, index) => 
                {
                    int count = vfxs.Take(index).Count(x => x.gameObject.name == r.gameObject.name);
                    return count > 0 ? $"{r.gameObject.name}_{count + 1}" : r.gameObject.name;
                })
                .ToArray();

            if (rendererNames.Length > 0)
            {
                // Получаем индекс выбранного элемента, устанавливаем по умолчанию первый
                int selectedIndex = vfxs.FindIndex(r => r.GetInstanceID() == _node._componentId);
                selectedIndex = Mathf.Max(selectedIndex, 0);  // По умолчанию выбираем первый элемент

                // Отображаем выпадающий список и обновляем индекс
                int newSelectedIndex = EditorGUILayout.Popup("VFX", selectedIndex, rendererNames);
                
                _node.SetComponent(vfxs[selectedIndex]);
                
                // Если индекс изменился, обновляем компонент
                if (newSelectedIndex != selectedIndex)
                {
                    selectedIndex = newSelectedIndex;
                    _node.SetComponent(vfxs[selectedIndex]);  // Обновляем компонент с новым выбранным значением
                    serializedObject.ApplyModifiedProperties();
                    NodeEditorWindow.current.Repaint();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No VFX nodes available in the graph.", MessageType.Warning);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
