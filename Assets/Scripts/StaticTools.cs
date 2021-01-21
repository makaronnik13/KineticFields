using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticTools 
{
    public static Texture2D ToTexture2D(RenderTexture rTex, int width, int heigth)
    {
        Texture2D tex = new Texture2D(width, heigth, TextureFormat.RGBAHalf, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public static Texture2D FlipTexture(this Texture2D original)
    {
        int textureWidth = original.width;
        int textureHeight = original.height;

        Color[] colorArray = original.GetPixels();

        for (int j = 0; j < textureHeight; j++)
        {
            int rowStart = 0;
            int rowEnd = textureWidth - 1;

            while (rowStart < rowEnd)
            {
                Color hold = colorArray[(j * textureWidth) + (rowStart)];
                colorArray[(j * textureWidth) + (rowStart)] = colorArray[(j * textureWidth) + (rowEnd)];
                colorArray[(j * textureWidth) + (rowEnd)] = hold;
                rowStart++;
                rowEnd--;
            }
        }

        Texture2D finalFlippedTexture = new Texture2D(original.width, original.height);
        finalFlippedTexture.SetPixels(colorArray);
        finalFlippedTexture.Apply();

        return finalFlippedTexture;
    }

}
