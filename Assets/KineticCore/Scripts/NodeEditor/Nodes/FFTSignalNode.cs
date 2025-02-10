using KineticFields;
using UnityEngine;
using XNode;
using UniRx;
using System;
using Zenject;
using System.Collections.Generic;
using System.Linq;

[NodeTint(0.2f, 0.3f, 0.1f)]
public class FFTSignalNode : Node
{
    [Range(0f, 1f)]
    [SerializeField] public float startSpectrumGap;
    [Range(0f, 1f)]
    [SerializeField] public float endSpectrumGap;

    public FrequencyGap SpectrumGap;
    
    
    [HideInInspector]
    public float[] OutputSpectrum;
    private IDisposable _subscription;
    private float v; 
    private float relaxationSpeed; 
    private float gainSpeed;
    private List<float> _values;

    [Output] public float output;
    
    public void SetValues(List<float> data)
    {
        if (data != null && data.Count > 0)
        {
            int startIdx = Mathf.FloorToInt(startSpectrumGap * data.Count);
            int endIdx = Mathf.FloorToInt(endSpectrumGap * data.Count);
            OutputSpectrum = data.GetRange(startIdx, endIdx - startIdx).ToArray();
        }
    }

    public override void StartNode()
    {
        base.StartNode();
        _subscription = Observable.EveryUpdate().Subscribe(_ =>
        {
            Update();
        });
    }

    private void Update()
    {
        if (OutputSpectrum == null || OutputSpectrum.Length == 0)
        {
            output = 0f;
        }
        else
        {
            output = OutputSpectrum.Max();
        }
        
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
    
    public override object GetValue(NodePort port)
    {
        return output;
    }

}

