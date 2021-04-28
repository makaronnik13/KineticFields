﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CurvesStorage
{
    public List<CurveInstance> Curves = new List<CurveInstance>();

    private Dictionary<string, CurveInstance> dict = new Dictionary<string, CurveInstance>();


    public CurvesStorage()
    {
        foreach (CurveInstance cI in Curves)
        {
            dict.Add(cI.Id, cI);
        }
    }



    public CurveInstance GetCurve(string id)
    {
        if (dict.Keys.Count == 0)
        {
            foreach (CurveInstance cI in Curves)
            {
                dict.Add(cI.Id, cI);
            }
        }

        if (dict.ContainsKey(id))
        {
            return dict[id];
        }

        return Curves[0];
    }

    public void AddCurve(UnityEngine.AnimationCurve curve)
    {
        CurveInstance cI = new CurveInstance(curve);
        Curves.Add(cI);
        dict.Add(cI.Id, cI);
    }

    public void AddCurve(CurveInstance newCurve)
    {
        Curves.Add(newCurve);
        dict.Add(newCurve.Id, newCurve);
    }
}