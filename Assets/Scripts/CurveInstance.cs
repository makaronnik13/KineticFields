using System;
using UnityEngine;

[Serializable]
public class CurveInstance
{
    [NonSerialized]
    public Action OnEdited = () => { };

    [SerializeField]
    private SerializingCurve curve;

    public AnimationCurve Curve
    {
        get
        {
            return curve.GetAnimationCurve();
        }
        set
        {
            curve = new SerializingCurve(value);
        }
    }
    public string Id;


    public CurveInstance(AnimationCurve curve)
    {
        Id = System.Guid.NewGuid().ToString();
        Curve = curve;
    }
}