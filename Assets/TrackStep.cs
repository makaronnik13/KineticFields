using com.armatur.common.flags;
using System;
using UnityEngine;

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
            HasKey.SetState(Position!=Vector2.zero);
        }
    }

    public GenericFlag<float> Time = new GenericFlag<float>("Time", 0);
    public GenericFlag<bool> HasKey = new GenericFlag<bool>("HasKey", false);


    public TrackStep(float time, Vector2 position)
    {
        Position = position;
        Time.SetState(time);
    }
}