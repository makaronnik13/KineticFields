using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using XNode;
using UniRx;
using UnityEngine.Serialization;

namespace OutputNodes
{
    [NodeWidth(300)]
    [NodeTint(0.6f, 0.15f, 0.2f)]
    public class PostProcessVolumeNode : Node, IUpdatableNode
    {
        public VolumeProfile Volume; 
        public List<PostProcessVolumePropertyData> properties = new();
        [Input(dynamicPortList = true)] public float[] portValues;

        private string _portName;
        private VolumeComponent _volumeComponent;
        private VolumeParameter _volumeParameter;
        
        // Find the effect in the VolumeProfile by its name
        private VolumeComponent FindEffectByName(string effectName)
        {
            if (string.IsNullOrEmpty(effectName)) return null;

            foreach (var component in Volume.components)
            {
                if (component.GetType().Name == effectName)
                {
                    return component;
                }
            }
            return null;
        }

        private float GetPortValue(int portId)
        {
            _portName = $"portValues[{portId}]";
            return GetInputPort(_portName).GetInputValue<float>();
        }
        
        // Find the parameter within the effect by its name
        private VolumeParameter FindParameterByName(VolumeComponent effect, string parameterName)
        {
            if (effect == null) return null;

            var fields = effect.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType.IsSubclassOf(typeof(VolumeParameter)) && field.Name == parameterName)
                {
                    return (VolumeParameter)field.GetValue(effect);
                }
            }
            return null;
        }

        // Sets the value of the parameter based on its type
        private void SetParameterValue(VolumeParameter parameter, float value)
        {
            switch (parameter)
            {
                case FloatParameter floatParameter:
                    floatParameter.value = (float)value;
                    break;
                case IntParameter intParameter:
                    intParameter.value = (int)value;
                    break;
                case BoolParameter boolParameter:
                    boolParameter.value = value>0 ? true:false;
                    break;
                default:
                    Debug.LogWarning("Unsupported VolumeParameter type: " + parameter.GetType().Name);
                    break;
            }
        }

        public void UpdateNode()
        {
            if (Volume == null) return;
            
            for (int i = 0; i < properties.Count; i++)
            {
               _volumeComponent = FindEffectByName( properties[i].EffectName);

                if (_volumeComponent == null)
                {
                    Debug.LogWarning($"Effect { properties[i].EffectName} not found!");
                    continue;
                }

                _volumeParameter = FindParameterByName(_volumeComponent,  properties[i].ParameterName);
                if (_volumeParameter != null)
                {
                    SetParameterValue(_volumeParameter, GetPortValue(i));
                }
            }
        }
    }
}
