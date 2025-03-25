
using UnityEngine;

[System.Serializable]
public class SerializedGradientKey
{
    public bool IsColor;
    public float time;
    public float a, b, c;

    public Color Color
    {
        get
        {
            return new Color(a,b,c,1);
        }
    }

    public SerializedGradientKey(float time, float alpha)
    {
        this.time = time;
        this.a = alpha;
        IsColor = false;
    }

    public SerializedGradientKey(float time, float r, float g, float b)
    {
        this.time = time;
        this.a = r;
        this.b = g;
        this.c = b;
        IsColor = true;
    }
}