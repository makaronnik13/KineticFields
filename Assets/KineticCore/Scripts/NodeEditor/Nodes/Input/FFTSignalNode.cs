using KineticFields;
using UnityEngine;
using XNode;
using UniRx;
using System;
using Zenject;
using System.Collections.Generic;
using System.Linq;

namespace InputNode
{
    [NodeTint(0.2f, 0.3f, 0.1f)]
    public class FFTSignalNode : Node, IUpdatableNode<List<float>>
    {
        [Range(0f, 1f)]
        [SerializeField] public float startSpectrumGap;
        [Range(0f, 1f)]
        [SerializeField] public float endSpectrumGap;
    
        public FrequencyGap SpectrumGap;
        
        [HideInInspector]
        public float[] OutputSpectrum;
        private float v; 
        private float relaxationSpeed; 
        private float gainSpeed;
        private List<float> _values;
    
        [Output] public float output;
        
        public override object GetValue(NodePort port)
        {
            return output;
        }

        public void UpdateNode(List<float> data)
        {
            if (data != null && data.Count > 0)
            {
                int startIdx = Mathf.FloorToInt(startSpectrumGap * data.Count);
                int endIdx = Mathf.FloorToInt(endSpectrumGap * data.Count);
                OutputSpectrum = data.GetRange(startIdx, endIdx - startIdx).ToArray();
            }
            
            if (OutputSpectrum == null || OutputSpectrum.Length == 0)
            {
                output = 0f;
            }
            else
            {
                output = OutputSpectrum.Max();
            }
        }
    }

}

