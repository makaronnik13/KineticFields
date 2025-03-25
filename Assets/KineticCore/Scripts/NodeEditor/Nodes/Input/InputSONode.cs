using KineticFields;
using UnityEngine;
using XNode;
using UniRx;
using System;
using Zenject;

namespace InputNode
{
    [NodeTint(0.1f, 0.3f, 0.1f)]
    public class InputSONode : Node, IUpdatableNode
    {
        [SerializeField] public InputSO inputSO;

        [Output] public float signalValue;

        private IDisposable _subscription;

        private InputSO _inputInstance;

        public void InitInstance(InputSO instance)
        {
            _inputInstance = instance;
        }

        public override object GetValue(NodePort port)
        {
            return _inputInstance.Value.Value;
        }

        public void UpdateNode()
        {
            signalValue = _inputInstance.Value.Value;
        }
    }
}
