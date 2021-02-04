using System;
using System.Collections.Generic;

[Serializable]
public class KineticPresetLine
{
    public KineticPreset[] Presets = new KineticPreset[10];

    public List<float> Spectrum = new List<float>();

    public string LineName;

    public KineticPresetLine()
    {

    }

    public KineticPresetLine(string name)
    {
        LineName = name;
        Presets = new KineticPreset[10];
        Spectrum = new List<float>();
    }
}