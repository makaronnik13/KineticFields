using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifyingParameter: ICloneable
{
    public float MIN, MAX;
    public GenericFlag<float> BaseValue = new GenericFlag<float>("BaseValue", 0);
    public GenericFlag<float> Value = new GenericFlag<float>("Value", 0);
    public GenericFlag<float> Multiplicator = new GenericFlag<float>("Multiplicator", 0.5f);

    public GenericFlag<int> SourceId = new GenericFlag<int>("SourceId", -1);

    public ISource Source
    {
        get
        {
            if (SourceId.Value == -1)
            {
                return null;
            }
            return KineticFieldController.Instance.Sources[SourceId.Value];
        }
        set
        {
            SetSource(value);
        }
    }

    public void Init()
    {
        SourceId.AddListener(SourceChanged);
        BaseValue.AddListener(BaseValueChanged);
        SetSource(Source);
    }

    public ModifyingParameter()
    {
     

    }

    public ModifyingParameter(float value, float min, float max, ISource source = null)
    {
        MIN = min;
        MAX = max;
        BaseValue.SetState(value);
        SourceId.SetState(KineticFieldController.Instance.Sources.IndexOf(source));

    }

    private void SourceChanged(int sourceId)
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        float v = BaseValue.Value;
        if (Source != null)
        {
            v += Source.SourceValue * Multiplicator.Value;
        }
        v = Mathf.Clamp(v, MIN, MAX*2f);
        Value.SetState(v);
    }

    private void MultiplyerValueChanged(float v)
    {
        Multiplicator.SetState(v);
    }

    public void SliderValueChanged(float v)
    {
        BaseValue.SetState(v);
    }

    public void SetSource(ISource source)
    {

        if (this.Source != null)
        {
            this.Source.OnValueChanged -= SourceValueChanged;
        }
        SourceId.SetState(KineticFieldController.Instance.Sources.IndexOf(source));
        if (this.Source!= null)
        {
            this.Source.OnValueChanged += SourceValueChanged;
        }


    }

    private void SourceValueChanged(float v)
    {
        UpdateValue();
    }

    private void BaseValueChanged(float v)
    {
        UpdateValue();
    }


    public void SetValue(float v)
    {
        BaseValue.SetState(Mathf.Clamp(v,MIN, MAX));
    }

    public object Clone()
    {

        Debug.Log(SourceId.Value);
        ModifyingParameter mp = new ModifyingParameter(BaseValue.Value, MIN, MAX, Source);

        mp.Multiplicator.SetState(Multiplicator.Value);
        mp.BaseValue.SetState(BaseValue.Value);
        Debug.Log("clone "+mp.Source+" "+ mp.SourceId.Value);

        mp.Init();
        return mp;
    }
}
