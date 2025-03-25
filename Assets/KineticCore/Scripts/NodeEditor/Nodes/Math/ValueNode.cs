using UnityEngine;

namespace MathNodes 
{
    [NodeWidth(175)]
    public class ValueNode : XNode.Node {
        [Output] public float value;
        public float Value;
        
        public override object GetValue(XNode.NodePort port) {
            return Value;
        }
    }
}