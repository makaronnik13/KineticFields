using KineticFields;
using UnityEngine;
using XNode;
using UniRx;
using System;
using Zenject;
using System.Collections.Generic;
using System.Linq;

[NodeWidth(300)]
public class RelaxationNode : Node, IDisposable
{
    public float relaxationTime = 1f;
    public float gainTime = 0.5f;

    [Output] public float outputValue;
    [Input] public float inputValue;
    
    private float v; 
    public float currentValue; 

    private float relaxationSpeed; 
    private float gainSpeed; 
    
    private IDisposable _subscription;
    

    public override void StartNode()
    {
        currentValue = 0f;
        
        relaxationSpeed = Mathf.Pow(1f / relaxationTime, 2); 
        gainSpeed = Mathf.Pow(1f / gainTime, 2);

        if (_subscription != null)
        {
            _subscription.Dispose();
        }
        
        _subscription = Observable.EveryUpdate().Subscribe(_ =>
        {
            Update();
        });
    }

    private void Update()
    {
        inputValue = GetInputValue<float>("inputValue", 0f);
        
        if (outputValue < inputValue)
        {
            outputValue = Mathf.Lerp(outputValue, inputValue, Time.deltaTime /relaxationTime);
        }
        else if (outputValue >  inputValue)
        {
            outputValue = Mathf.Lerp(outputValue, inputValue, Time.deltaTime /gainTime);
        }
        
       
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}


