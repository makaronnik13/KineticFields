using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializingKeyframe
{
    [SerializeField]
    public float time;
    [SerializeField]
    public float value;
    [SerializeField]
    public float inTangent;
    [SerializeField]
    public float inWeight;
    [SerializeField]
    public float outTangent;
    [SerializeField]
    public float outWeight;
    [SerializeField]
    public int tangentMode;
    [SerializeField]
    public WeightedMode weightedMode;


    public SerializingKeyframe(Keyframe kf)
    {
        time = kf.time;
        value = kf.value;
        inTangent = kf.inTangent;
        inWeight = kf.inWeight;
        outTangent = kf.outTangent;
        outWeight = kf.outWeight;
        tangentMode = kf.tangentMode;
        weightedMode = kf.weightedMode;
    }

}
