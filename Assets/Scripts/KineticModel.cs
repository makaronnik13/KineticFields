using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Model", menuName = "KineticModel")]
public class KineticModel : ScriptableObject
{
    public VisualEffectAsset AnimationGraph, KinectGraph;
}
