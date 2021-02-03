using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GradientStorage
{
    public List<GradientInstance> Gradients = new List<GradientInstance>();

    public GradientInstance GetGradient(string id)
    {
        GradientInstance gradient = Gradients.FirstOrDefault(c => c.Id == id);
        if (gradient == null)
        {
            gradient = Gradients.FirstOrDefault();
        }

        return gradient;
    }
}