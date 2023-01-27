using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KineticFields;
using Lasp;
using UniRx;
using UnityEngine;
using Zenject;

public class SignalSource : BaseSignalSource
{
    [SerializeField] private FrequencyGap gap;
    [SerializeField] private SignalType signalType;
    [SerializeField] private bool interpolate;
    [SerializeField] public float multiplyer = 1;
    [SerializeField] private float interpolaionTime;
    [SerializeField] private float extraValue;

    [HideInInspector]
    [SerializeReference] PropertyBinder[] propertyBinders = null;
    public PropertyBinder[] PropertyBinders
    { get => (PropertyBinder[])propertyBinders.Clone();
        set => propertyBinders = value; }
    
    public float Value => value;
    public float MultipliedValue => Value * multiplyer+extraValue;
    
    private float value;

    private FFTService fftService;
    private float max;
    private float average;
    private float floatingValue;
    private float dif;
    private float lastValue;
    private float v;

    public float V => v;


    [Inject]
    public void Construct(FFTService fftService)
    {
        this.fftService = fftService;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            float[] data = GetSpectrumData();

            if (data.Length>0)
            {
                max = GetSpectrumData().Max();
                average = GetSpectrumData().Average();
                
                if (signalType == SignalType.Max || signalType == SignalType.MaxDiff)
                {
                    v = max;
                }
                if (signalType == SignalType.Average || signalType == SignalType.AverageDiff)
                {
                    v = average;
                }

                if (interpolate)
                {
                    v = Mathf.Lerp(value, v, Time.deltaTime / interpolaionTime);   
                }
            
            
            
                if (signalType == SignalType.AverageDiff || signalType == SignalType.MaxDiff)
                {
                    value =  v - lastValue;
                }
                else
                {
                    value = v;
                }

                Signal.Value = v;
                lastValue = v;
   
                if (propertyBinders != null)
                    foreach (var b in propertyBinders) b.Level = value;
            }
        }).AddTo(this);
    }

    public float[] GetSpectrumData()
    {
        if (fftService == null)
        {
            return new float[0];
        }
        
        return fftService.GetSpectrumGap(gap);
    }
    
    public enum SignalType
    {
        Max,
        Average,
        MaxDiff,
        AverageDiff
    }
}
