using System;
using XNode;
using UniRx;
using UnityEngine;

namespace KineticNodes
{
    [NodeWidth(200)]
    public class CollectNode : Node, IUpdatableNode
    {
        [Output] public float outputValue;
        [Input] public float inputValue;

        public override void StartNode()
        {
            outputValue = 0f;
        }
        
        public override object GetValue(NodePort port)
        {
            return outputValue;
        }

        public void UpdateNode()
        {
            inputValue = GetInputValue<float>("inputValue", 0f);
            outputValue += inputValue;
        }
    }
}
