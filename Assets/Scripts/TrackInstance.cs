using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackInstance
{
    [SerializeField]
    private SerializedGradientKey _color;

    [SerializeField]
    public GenericFlag<int> CurrentRepeat = new GenericFlag<int>("CurrentRepeat", 0);

    public Color Color
    {
        get
        {
            return new Color(_color.a, _color.b, _color.c);
        }
        set
        {
            _color = new SerializedGradientKey(0, value.r, value.g, value.b);
        }
    }

    public PointTrack PositionTrack;
    public PointTrack RadiusTrack;

   [SerializeField]
    private int _iconId = 0;

    public GenericFlag<int> Size = new GenericFlag<int>("Size", 0);
    public GenericFlag<int> RepeatCount = new GenericFlag<int>("RepeatCount", 1);


    public Sprite Icon
    {
        get
        {
            return DefaultResources.TrackSprites[_iconId];
        }
        set
        {
            _iconId = DefaultResources.TrackSprites.IndexOf(value);
        }
    }

    public int Steps
    {
        get
        {
            return Mathf.RoundToInt(Mathf.Pow(2, 1 + Size.Value));
        }
    }

    public TrackInstance()
    {
        CurrentRepeat = new GenericFlag<int>("CurrentRepeat", 0);
    }

    public TrackInstance(Color color, Sprite icon)
    {
        CurrentRepeat = new GenericFlag<int>("CurrentRepeat", 0);
        Color = color;
        Icon = icon;
        PositionTrack = new PointTrack(PointTrack.TrackType.Position);
        RadiusTrack = new PointTrack(PointTrack.TrackType.Radius);
    }
}