using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PresetPath
{
    [SerializeField]
    private string presetId;
    public KineticPreset preset
    {
        get
        {
            return KineticFieldController.Instance.Session.Value.GetPresetById(presetId);
        }
    }

    [SerializeField]
    
    public List<SerializedVector> Points;

    public PresetPath(List<Vector2> vector2s)
    {
        this.Points = new List<SerializedVector>();
        foreach (Vector2 point in vector2s)
        {
            this.Points.Add(new SerializedVector(point.x, point.y, 0));
        }
    }
}
