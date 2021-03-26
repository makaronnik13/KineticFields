﻿using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackInstance
{
    [SerializeField]
    private SerializedGradientKey _color;
    
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

    public List<PointTrack> PointsTracks = new List<PointTrack>();

   [SerializeField]
    private int _iconId = 0;

    public GenericFlag<int> Size = new GenericFlag<int>("Size", 0);

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

    }

    public TrackInstance(Color color, Sprite icon)
    {
        Color = color;
        Icon = icon;
    }
}