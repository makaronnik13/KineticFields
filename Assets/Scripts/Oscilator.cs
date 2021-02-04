﻿using System;
using UnityEngine;

[Serializable]
public class Oscilator : ISource
{
    public Oscilator(float multiplyer, int repeatRate)
    {
        this.Multiplyer = multiplyer;
        this.RepeatRate = repeatRate;
    }

    public Sprite Icon
    {
        get
        {
            return DefaultResources.OscilatorSprites[KineticFieldController.Instance.Session.Value.Oscilators.IndexOf(this)];
        }
    }

    [NonSerialized]
    private Action<float> onValueChanged = (v) => { };
    public Action<float> OnValueChanged
    {
        get
        {
            return onValueChanged;
        }
        set
        {
            onValueChanged = value;
        }
    }

    [NonSerialized]
    public Action<float> onTimeChanged = (v) => { };
    [NonSerialized]
    public Action<float> onMiddleValueChanged = (v) => { };

    private float value;
    public float SourceValue
    {
        get
        {
            return value;
        }
    }

    public float Multiplyer = 1;
    public int RepeatRate = 0;

    [SerializeField]
    private string curveId = "";
    public CurveInstance Curve
    {
        get
        {
            return SessionsManipulator.Instance.Curves.GetCurve(curveId);
        }
        set
        {
            curveId = value.Id;
        }
    }

    private float startBeatTime = 0;
    private int beatsCount = 0;

    public void UpdateOscilator()
    {
        float repeatTime = 60f / BpmManager.Instance.Bpm.Value;

        if (RepeatRate>0)
        {
            repeatTime /= Mathf.Pow(2, RepeatRate);
        }
        else
        {
            repeatTime *= Mathf.Pow(2, Mathf.Abs(RepeatRate));
        }

        float timeLast = Time.realtimeSinceStartup - startBeatTime;

        float v = timeLast / repeatTime;

        if (RepeatRate > 0)
        {
            v *= Mathf.Pow(2, RepeatRate);
            while (v>1f)
            {
                v--;
            }
        }

        onTimeChanged(v);

        
        value = Curve.Curve.Evaluate(v);

        onMiddleValueChanged(value);
        value*=Multiplyer;
        OnValueChanged(value);
    }



    public void Beat()
    {
        beatsCount++;

        if (RepeatRate<0)
        {
            if (beatsCount>= Mathf.Pow(2, Mathf.Abs(RepeatRate)))
            {
                Reset();
            }
        }
        else
        {
            Reset();
        }
    }

    public void Reset()
    {
        beatsCount = 0;
        startBeatTime = Time.realtimeSinceStartup;
    }
}