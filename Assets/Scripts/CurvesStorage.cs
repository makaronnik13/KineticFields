using System;
using System.Collections.Generic;
using System.Linq;

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
}