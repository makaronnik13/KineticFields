using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedGradient
{

    public GradientMode mode;

    public List<SerializedGradientKey> keys = new List<SerializedGradientKey>();

    public SerializedGradient(Gradient gradient)
    {
        mode = gradient.mode;
        keys = new List<SerializedGradientKey>();
        foreach (GradientAlphaKey key in gradient.alphaKeys)
        {
            keys.Add(new SerializedGradientKey(key.time, key.alpha));
        }
        foreach (GradientColorKey key in gradient.colorKeys)
        {
            keys.Add(new SerializedGradientKey(key.time, key.color.r, key.color.g, key.color.b));
        }
    }

    public Gradient GetGradient()
    {
        Gradient gradient = new Gradient();
        gradient.mode = mode;
        List<GradientAlphaKey> aKeys = new List<GradientAlphaKey>();
        List<GradientColorKey> cKeys = new List<GradientColorKey>();

        foreach (SerializedGradientKey key in keys)
        {
            if (key.IsColor)
            {
                cKeys.Add(new GradientColorKey(new Color(key.a, key.b, key.c), key.time));
            }
            else
            {
                aKeys.Add(new GradientAlphaKey(key.a, key.time));
            }
        }

        gradient.alphaKeys = aKeys.ToArray();
        gradient.colorKeys = cKeys.ToArray();

        return gradient;
    }
}