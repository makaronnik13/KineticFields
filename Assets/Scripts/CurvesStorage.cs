using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CurvesStorage
{
    public List<CurveInstance> Curves = new List<CurveInstance>();

    public CurveInstance GetCurve(string id)
    {
        CurveInstance curve = Curves.FirstOrDefault(c=>c.Id == id);
        if (curve == null)
        {
            curve = Curves.FirstOrDefault();
        }

        return curve;
    }
}