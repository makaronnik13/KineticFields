using XNode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace OutputNodes
{
    [NodeWidth(400)]
    [NodeTint(0.6f, 0.15f, 0.2f)]
    public class VFXPropertyNode : Node, IUpdatableNode
    {
        [Input] public VisualEffect visualEffect = null;
        [SerializeReference] public List<string> properties = new List<string>();
        [Input(dynamicPortList = true)] public float[] portValues;

        private string _portName;
        
        public void UpdateNode()
        {
            if (visualEffect == null)
            {
                visualEffect = GetInputValue<VisualEffect>("visualEffect");
            }
            
            if (visualEffect == null || properties.Count == 0) return;

            // Применяем настройки к материалу
            for (int i = 0; i <  properties.Count; i++)
            {
                visualEffect.SetFloat(properties[i], GetPortValue(i));
            }
        }
        
        private float GetPortValue(int portId)
        {
            _portName = $"portValues[{portId}]";
            return GetInputPort(_portName).GetInputValue<float>();
        }
    }
}