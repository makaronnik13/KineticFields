using System;
using UnityEngine;

[Serializable]
public class GradientInstance
{
    [NonSerialized]
    public Action OnEdited = () => { };

    private SerializedGradient gradient;
    public Gradient Gradient
    {
        get
        {
            return gradient.GetGradient();
        }
        set
        {
            gradient = new SerializedGradient(value);
        }
    }
    public string Id;

    public GradientInstance(Gradient gradient)
    {
        Id = System.Guid.NewGuid().ToString();
        Gradient = gradient;
    }
}