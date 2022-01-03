
using Assets.WasapiAudio.Scripts.Core;

[System.Serializable]
public class LoopBackSettings
{
    public int SpectrumSize = 512;
    public ScalingStrategy ScalingStrategy = ScalingStrategy.Linear;
    public int MinFrequency = 0;
    public int MaxFrequency = 12000;

}