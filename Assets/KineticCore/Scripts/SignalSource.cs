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
    [SerializeField] private float interpolaionTime;

    [HideInInspector]
    [SerializeReference] PropertyBinder[] propertyBinders = null;
    public PropertyBinder[] PropertyBinders
    { get => (PropertyBinder[])propertyBinders.Clone();
        set => propertyBinders = value; }
    
    //public float Value => value;
    //public float MultipliedValue => Value * multiplyer+extraValue;
    
    //private float value;

    private FFTService fftService;
    private float max;
    private float average;
    private float floatingValue;
    private float dif;
    private float lastValue;
    private float v;

    public float V => v;

    private List<float> data;

    [Inject]
    public void Construct(FFTService fftService)
    {
        this.fftService = fftService;
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        data = GetSpectrumData().ToList();

        if (data!=null && data.Count > 0)
        {
            max = data.Max();
            average = data.Average();

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
                if (Signal.Value>v)
                {
                    v = Mathf.Lerp(Signal.Value, v, Time.deltaTime / interpolaionTime);
                }
                else
                {
                    if (v>0)
                    {
                        v -= Time.deltaTime / interpolaionTime;
                    }
                }
            }



            if (signalType == SignalType.AverageDiff || signalType == SignalType.MaxDiff)
            {
                Signal.Value = v - lastValue;
            }
            else
            {
                Signal.Value  = v;
            }

            lastValue = v;

            if (propertyBinders != null)
                foreach (var b in propertyBinders) b.Level = MultipliedSignal.Value;
        }
    }

    public IEnumerable<float> GetSpectrumData()
    {
        if (fftService == null)
        {
            return null; // new float[0];
        }
        
        return fftService.GetCachedSpectrumGap(gap);
    }
    
    public enum SignalType
    {
        Max,
        Average,
        MaxDiff,
        AverageDiff
    }
}
