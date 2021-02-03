using System;
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

    public Action<float> onTimeChanged = (v) => { };
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

    public void UpdateOscilator(float v)
    {
        v /= Mathf.Pow(2,RepeatRate);

        onTimeChanged(v);

        while (v>1f)
        {
            v -= 1f;
        }
        value = Curve.Curve.Evaluate(v);
        onMiddleValueChanged(value);
        value*=Multiplyer;
        OnValueChanged(value);
    }
}