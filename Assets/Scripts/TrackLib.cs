using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackLib
{
    [SerializeField]
    public string Name;

    [SerializeField]
    public List<TrackInstance> Tracks = new List<TrackInstance>();

    [SerializeField]
    public GenericFlag<int> ChangeRate = new GenericFlag<int>("ChangeRate", 0);

    public TrackLib()
    {

    }

    public TrackLib(string name)
    {
        Name = name;
    }

}