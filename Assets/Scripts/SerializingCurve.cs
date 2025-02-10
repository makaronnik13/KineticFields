using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializingCurve
{
    [NonSerialized]
    private AnimationCurve curve = null;

    [SerializeField]
    private List<SerializingKeyframe> keys = new List<SerializingKeyframe>();

    public SerializingCurve()
    {
        CreateCurve();
    }

    public SerializingCurve(AnimationCurve curve)
    {
        foreach (Keyframe kf in curve.keys)
        {
            keys.Add(new SerializingKeyframe(kf));
        }

        CreateCurve();
    }

    public void Update(AnimationCurve c)
    {
        keys.Clear();
        foreach (Keyframe kf in c.keys)
        {
            keys.Add(new SerializingKeyframe(kf));
        }
        CreateCurve();
    }

    private void CreateCurve()
    {
        curve = new AnimationCurve();
        foreach (SerializingKeyframe kf in keys)
        {
            Keyframe newKey = new Keyframe(kf.time, kf.value);
            newKey.inTangent = kf.inTangent;
            newKey.inWeight = kf.inWeight;
            newKey.outTangent = kf.outTangent;
            newKey.tangentMode = kf.tangentMode;
            newKey.time = kf.time;
            newKey.value = kf.value;
            newKey.weightedMode = kf.weightedMode;
            newKey.outWeight = kf.outWeight;
            curve.AddKey(newKey);
        }
    }

    public AnimationCurve GetAnimationCurve()
    {
        if (curve == null)
        {
            CreateCurve();
        }
        return curve;
    }

}
