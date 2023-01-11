using System;
using UnityEngine;
using UnityEngine.VFX;

public class VisualisationParameter
{
    private VisualEffect visual;

    public string ParameterName;
    public float Value
    {
        get
        {
            if (gap!=null)
            {
                return value; //gap.Value + value;
            }
            return value;
        }
    }

    private FrequencyGap gap;

    private float value = 0;

    public VisualisationParameter(string Name, VisualEffect visual)
    {
        this.visual = visual;
        ParameterName = Name;
    }

    public void SetValue(float v)
    {
        value = v;
    }

    public void ConnectGap(FrequencyGap gap)
    {
        if (this.gap!=null)
        {
            //this.gap.OnValueChanged -= ValueChanged;
        }
        this.gap = gap;
        if (gap != null)
        {
            //gap.OnValueChanged += ValueChanged;
        }
    }

    private void ValueChanged(float v)
    {
        if (visual)
        {
            visual.SetFloat(ParameterName, v);
        }
    }
}