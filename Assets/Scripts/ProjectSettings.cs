using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectSettings", fileName = "ProjectSettings")]
public class ProjectSettings: ScriptableObject
{
    [GradientUsage(true)]
    public List<Gradient> Gradients;
    public List<AnimationCurve> SizeCurves;
    public List<Mesh> Meshes;

    public AnimationCurve PointsCountCurve;



    public int ThresholdWindowSize = 15;
    public float ThresholdMultiplier = 2f;

    public int GetCount(float lifetime)
    {
        return Mathf.RoundToInt(PointsCountCurve.Evaluate(lifetime));
    }
}