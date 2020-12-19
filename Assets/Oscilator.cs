using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscilator : MonoBehaviour, ISource
{
    public Sprite icon;
    public AnimationCurve Curve;

    float t = 0;
    private AnimationCurve curve;

    public Sprite Icon => icon;

    private Action<float> onValueChanged = (v)=> { };
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

    public float SourceValue
    {
        get
        {
            return curve.Evaluate(t);
        }
    }

    private void Start()
    {
        SetCurve(Curve);
    }

    private void SetCurve(AnimationCurve curve)
    {
        t = 0;
        this.curve = curve;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t>= curve.keys[Curve.keys.Length - 1].time)
        {
            t = 0;
        }
        onValueChanged.Invoke(curve.Evaluate(t));
    }
}
