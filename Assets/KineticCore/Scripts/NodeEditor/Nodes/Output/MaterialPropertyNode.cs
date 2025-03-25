using XNode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace OutputNodes
{
    [NodeWidth(400)]
    [NodeTint(0.6f, 0.15f, 0.2f)]
    public class MaterialPropertyNode : Node, IUpdatableNode
    {
        [Input] public Renderer renderer = null;
        [SerializeReference] public List<string> properties = new List<string>();
        [Input(dynamicPortList = true)] public float[] portValues;

        private string _portName;
        
        public void UpdateNode()
        {
            if (renderer == null)
            {
                renderer = GetInputValue<Renderer>("renderer");
            }
            
            if (renderer == null || properties.Count == 0) return;

            // Применяем настройки к материалу
            var prop = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(prop);
            
            for (int i = 0; i <  properties.Count; i++)
            {
                prop.SetFloat(properties[i], GetPortValue(i));
            }
            
            renderer.SetPropertyBlock(prop);
        }
        
        private float GetPortValue(int portId)
        {
            _portName = $"portValues[{portId}]";
            return GetInputPort(_portName).GetInputValue<float>();
        }
    }
}