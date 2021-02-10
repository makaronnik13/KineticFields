using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StaticTools 
{
    public static Texture2D ToTexture2D(RenderTexture rTex, int width, int heigth)
    {
        RenderTexture rt = RenderTexture.active;

        Texture2D tex = new Texture2D(width, heigth, TextureFormat.RGBAHalf, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = rt;
        return tex;
    }

    public static Texture2D FlipTexture(this Texture2D original)
    {
        int textureWidth = original.width;
        int textureHeight = original.height;

        Color[] colorArray = original.GetPixels();
        Color[] newColors = new Color[colorArray.Length];


        List<Color> nc = new List<Color>(colorArray);
        nc.Reverse();

        //newColors = nc.ToArray();

        Debug.Log(textureWidth+"/"+textureHeight);

        for (int i = 0; i < textureHeight; i++)
        {
            for (int j = 0; j < textureWidth; j++)
            {
                if (i>=textureHeight/2f)
                {
                    newColors[i * textureWidth + j] = nc[i * textureWidth + j];
                }
                else
                {
                    newColors[i * textureWidth + j] = nc[i * textureWidth + (textureWidth-1-j)];
                }
            }
        }

        for (int j = 0; j < textureHeight; j++)
        {
            int rowStart = 0;
            int rowEnd = textureWidth - 1;

          

            while (rowStart < rowEnd)
            {
                Color hold = colorArray[(j * textureWidth) + (rowStart)];

               // Debug.Log(colorArray[(j * textureWidth) + (rowEnd)]);

                //newColors[(j * textureWidth) + (rowEnd)] = colorArray[(j * textureWidth) + (rowEnd)];

               // newColors[(j * textureWidth) + (rowEnd)] = colorArray[(j * textureWidth) + (rowEnd)];

                //colorArray[(j * textureWidth) + (rowStart)] = colorArray[(j * textureWidth) + (rowEnd)];
                //colorArray[(j * textureWidth) + (rowEnd)] = hold;
                rowStart++;
                rowEnd--;
            }
        }
        

        Texture2D finalFlippedTexture = new Texture2D(original.width, original.height, TextureFormat.RGBAHalf, false);
        finalFlippedTexture.SetPixels(newColors);
        finalFlippedTexture.Apply();

        return finalFlippedTexture;
    }

    public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, false);
    }

    public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, true, false);
    }

    public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, true);
    }

    static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
    {
        //list of all the unique key times
        var keysTimes = new List<float>();

        if (!noColor)
        {
            for (int i = 0; i < a.colorKeys.Length; i++)
            {
                float k = a.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.colorKeys.Length; i++)
            {
                float k = b.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        if (!noAlpha)
        {
            for (int i = 0; i < a.alphaKeys.Length; i++)
            {
                float k = a.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.alphaKeys.Length; i++)
            {
                float k = b.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
        GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

        //Pick colors of both gradients at key times and lerp them
        for (int i = 0; i < keysTimes.Count; i++)
        {
            float key = keysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            clrs[i] = new GradientColorKey(clr, key);
            alphas[i] = new GradientAlphaKey(clr.a, key);
        }

        var g = new UnityEngine.Gradient();
        g.SetKeys(clrs, alphas);

        return g;
    }
}
