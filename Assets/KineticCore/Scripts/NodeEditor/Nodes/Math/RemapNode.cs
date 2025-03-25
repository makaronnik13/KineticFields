using UnityEngine;
using XNode;

namespace MathNodes
{

    [NodeWidth(175)]
    public class RemapNode : Node
    {
        [Input] public float inputValue;

        public float inMin = 0f;
        public float inMax = 1f;
        public float outMin = 0f;
        public float outMax = 1f;

        [Output] public float outputValue;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outputValue")
            {
                float input = GetInputValue("inputValue", 0f);
                return Remap(input, inMin, inMax, outMin, outMax);
            }

            return null;
        }

        private float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return Mathf.Clamp(outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin), outMin, outMax);
        }

    }
}