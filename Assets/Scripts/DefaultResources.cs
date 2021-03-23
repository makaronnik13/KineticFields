using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DefaultResources
{
    private static ProjectSettings settings;
    public static ProjectSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = Resources.Load<ProjectSettings>("ProjectSettings");
            }
            return settings;
        }
    }

    private static List<Sprite> gapSprites;
    public static List<Sprite> GapSprites
    {
        get
        {
            if (gapSprites == null)
            {
                gapSprites = Resources.LoadAll<Sprite>("Sprites/Gaps").ToList();
            }
            return gapSprites;
        }
    }

    private static List<Sprite> presetSprites;
    public static List<Sprite> PresetSprites
    {
        get
        {
            if (presetSprites == null)
            {
                presetSprites = Resources.LoadAll<Sprite>("Sprites/Presets").ToList();
            }
            return presetSprites;
        }
    }

    public static List<Sprite> TrackSprites
    {
        get
        {
            return PresetSprites;
        }
    }

    private static List<Sprite> oscilatorSprites;
    public static List<Sprite> OscilatorSprites
    {
        get
        {
            if (oscilatorSprites == null)
            {
                oscilatorSprites = Resources.LoadAll<Sprite>("Sprites/Oscilators").ToList();
            }
            return oscilatorSprites;
        }
    }
}