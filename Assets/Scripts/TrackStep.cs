using com.armatur.common.flags;
using System;
using UnityEngine;

[Serializable]
public class TrackStep
{
    [SerializeField]
    private float x, y;

    public Vector2 Position
    {
        get
        {
            return new Vector2(x,y);
        }
        set
        {

            x = value.x;
            y = value.y;
        }
    }

    public float Radius;

    public GenericFlag<float> Time = new GenericFlag<float>("Time", 0);

    public TrackStep(float time, Vector2 position)
    {
        Position = position;
        Time.SetState(time);
    }

    public TrackStep(float time, float scale)
    {
        Radius = scale;
        Time.SetState(time);
    }
}