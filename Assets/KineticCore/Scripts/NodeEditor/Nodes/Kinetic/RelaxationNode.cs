using UnityEngine;
using XNode;
using UniRx;
using System;

namespace KineticNodes
{

    [NodeWidth(300)]
    public class RelaxationNode : Node, IUpdatableNode
    {
        public float relaxationTime = 1f;
        public float gainTime = 0.5f;

        [Output] public float outputValue;
        [Input] public float inputValue;

        public override object GetValue(NodePort port)
        {
            return outputValue;
        }

        public void UpdateNode()
        {
            inputValue = GetInputValue<float>("inputValue", 0f);

            if (outputValue < inputValue)
            {
                outputValue = Mathf.Lerp(outputValue, inputValue, Time.deltaTime / relaxationTime);
            }
            else if (outputValue > inputValue)
            {
                outputValue = Mathf.Lerp(outputValue, inputValue, Time.deltaTime / gainTime);
            }
        }
    }
}


