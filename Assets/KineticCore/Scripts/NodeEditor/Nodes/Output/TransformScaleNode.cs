using System;
using UnityEngine;
using XNode;
using UniRx;
using UnityEngine.Serialization;

namespace OutputNodes
{
    [NodeWidth(300)]
    [NodeTint(0.6f, 0.15f, 0.2f)]
    public class TransformScaleNode : Node, IUpdatableNode
    {
        [Input] [SerializeField] private Vector3 _scale;
        [Input] [SerializeField] private Transform _transform;

        private Transform  TransformInput  => GetInputValue<Transform>("_transform", null);
        private Vector3  ScaleInput  => GetInputValue<Vector3>("_scale", Vector3.zero);

        public void UpdateNode()
        {
            if (TransformInput == null)
            {
                return;
            }

            TransformInput.localScale = ScaleInput;
        }
    }
}