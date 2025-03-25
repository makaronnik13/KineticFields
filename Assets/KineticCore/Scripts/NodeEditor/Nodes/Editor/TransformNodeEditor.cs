using System.Collections.Generic;
using System.Linq;
using ComponentsNodes;
using UnityEditor;
using UnityEngine;

namespace KineticCore.Scripts.NodeEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(TransformNode))]
    public class TransformNodeEditor : XNodeEditor.NodeEditor
    {
        private KineticGraph _graph;
        private TransformNode _node;

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            _node = (TransformNode)target;
            _graph = _node.graph as KineticGraph;

            KineticSceneGraph sceneGraph = KineticGraphManager.GetKineticSceneGraph(_graph);

            if (sceneGraph == null)
            {
                return;
            }
        
            // Получаем список всех трансформов в графе
            List<Transform> transforms = sceneGraph.GetRefComponents<Transform>();

            // Создаём список для dropdown меню
            string[] transformNames = transforms.Select(tn => tn.name).ToArray();
        
            // Если список не пустой, показываем выпадающий список
            if (transformNames.Length > 0)
            {
                int selectedIndex = transforms.FindIndex(tn => tn.GetInstanceID() == _node._componentId);
                selectedIndex = Mathf.Max(selectedIndex, 0);  // По умолчанию выбираем первый элемент

                // Создаём выпадающий список для выбора transform
                selectedIndex = EditorGUILayout.Popup("Transform", selectedIndex, transformNames);

                // Обновляем transformID
                if (selectedIndex >= 0 && selectedIndex < transforms.Count)
                {
                    _node._componentId = transforms[selectedIndex].GetInstanceID();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Transform nodes available in the graph.", MessageType.Warning);
            }

            // Применяем изменения
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}