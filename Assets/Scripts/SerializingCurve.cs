using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializingCurve
{
    [SerializeField]
    private List<SerializingKeyframe> keys = new List<SerializingKeyframe>();

    public SerializingCurve(AnimationCurve curve)
    {
        foreach (Keyframe kf in curve.keys)
        {
            keys.Add(new SerializingKeyframe(kf));
        }
    }

    public AnimationCurve GetAnimationCurve()
    {
        AnimationCurve crv = new AnimationCurve();
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
            crv.AddKey(newKey);
        }

        return crv;
    }

}
