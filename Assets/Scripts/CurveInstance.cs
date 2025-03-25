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
            if (curve == null)
            {
                curve = new SerializingCurve(value);
            }
            else
            {
                curve.Update(value);
            }
        }
    }
    public string Id;


    public CurveInstance(AnimationCurve curve)
    {
        Id = System.Guid.NewGuid().ToString();
        Curve = curve;
    }

    public void Update(AnimationCurve pointCurve)
    {
        Curve = pointCurve;
    }
}