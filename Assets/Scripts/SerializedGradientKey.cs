
[System.Serializable]
public class SerializedGradientKey
{
    public bool Color;
    public float time;
    public float a, b, c;

    public SerializedGradientKey(float time, float alpha)
    {
        this.time = time;
        this.a = alpha;
        Color = false;
    }

    public SerializedGradientKey(float time, float r, float g, float b)
    {
        this.time = time;
        this.a = r;
        this.b = g;
        this.c = b;
        Color = true;
    }
}