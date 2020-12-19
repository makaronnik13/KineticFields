using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectSettings", fileName = "ProjectSettings")]
public class ProjectSettings: ScriptableObject
{
    [GradientUsage(true)]
    public List<Gradient> Gradients;
    public List<AnimationCurve> SizeCurves;
    public List<Texture2D> ColorMaps;
    public List<Texture2D> DisplacementMaps;
    public List<Mesh> Meshes;

    public AnimationCurve PointsCountCurve;

    public int GetCount(float lifetime)
    {
        return Mathf.RoundToInt(PointsCountCurve.Evaluate(lifetime));
    }
}