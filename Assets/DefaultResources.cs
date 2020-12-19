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
}