using UnityEngine;

namespace MathNodes
{
    [NodeWidth(175)]
    public class ClampNode : XNode.Node {
        [Input] public float input;
        [Output] public float output;

        public float Min = 0;
        public float Max = 1;
        
        public override object GetValue(XNode.NodePort port)
        {
            if (port.fieldName == "output")
            {
                float i = GetInputValue<float>("input", this.input);
                return Mathf.Clamp(i, Min, Max);;
            }

            return null;
        }
    }
}