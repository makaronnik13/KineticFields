using System;
using UnityEngine;
using XNode;
using UniRx;

namespace OutputNodes
{
    [NodeWidth(300)]
    [NodeTint(0.6f, 0.15f, 0.2f)]
    public class TransformRotationNode : Node, IUpdatableNode
    {
        private enum  RotationMode
        {
            Set,
            Rotate
        }

        [SerializeField] private RotationMode _mode;
        [Input] [SerializeField] private Vector3 _rotation;
        [Input] [SerializeField] private Transform _transform;

        private Transform TransformInput  => GetInputValue<Transform>("_transform", null);
        private Vector3 RotationInput  => GetInputValue<Vector3>("_rotation", Vector3.zero);

        public void UpdateNode()
        {
            if (TransformInput == null)
            {
                return;
            }

            switch (_mode)
            {
                case RotationMode.Rotate:
                    TransformInput.Rotate(RotationInput*Time.deltaTime, Space.Self);
                    break;
                case RotationMode.Set:
                    TransformInput.localRotation = Quaternion.Euler(RotationInput);
                    break;
            }
        }
    }
}
