using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Color = UnityEngine.Color;
using Graphics = UnityEngine.Graphics;

public class Texture3dPainter : MonoBehaviour
{
    public int Size = 1024;
    public RenderTexture renderTexture;
    //public Texture2D texture;
    private Texture3D texture3d;
    public Transform aim;
    //public Material material;
    private int index = 0;
    
        void Start () 
        {
            //texture = new Texture2D (1024, 1024, GraphicsFormat.R8G8B8_UInt, TextureCreationFlags.None);
        }
        
        void Update ()
        {
            
            // Lazy initialization of the temporary texture
            if (texture3d == null)
                texture3d = new Texture3D(Size, Size, 1, TextureFormat.R8, false);

            // Texture update
           /* index++;
            if (index>=texture.width*texture.height)
            {
                index = 0;
            }*/
            
            texture3d.SetPixel(Mathf.RoundToInt(aim.position.x), Mathf.RoundToInt(aim.position.y), Mathf.RoundToInt(aim.position.z), Color.white);
            texture3d.Apply();

            // Update the external render texture.
            if (renderTexture != null)
                Graphics.CopyTexture(texture3d, renderTexture);
        }
        
}
