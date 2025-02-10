using KineticFields;
using UnityEngine;
using XNode;
using UniRx;
using System;
using Zenject;

[NodeTint(0.1f, 0.3f, 0.1f)]
public class InputSONode: Node  
{
    [SerializeField] public InputSO inputSO;

    [Output] public float signalValue;

    private IDisposable _subscription;

    private InputSO _inputInstance;

    [Inject] private KineticInputService _inputService;
    public void Construct(KineticInputService inputService)
    {
        _inputService = inputService;
        Debug.Log("Construct");
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);
    }

    protected override void Init()
    {
        base.Init();
    }

    public void InitInstance(InputSO instance)
    {
        _inputInstance = instance;
    }

    public override void StartNode()
    {
        if (_subscription != null)
        {
            _subscription.Dispose();
        }

        _subscription = _inputInstance.Value.Subscribe(value =>
        {
            signalValue = value;
        });
    }

   

    public override object GetValue(NodePort port)
    {
        return signalValue;
    }

}
