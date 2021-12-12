using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

    public class CutoutMaskUI : Image 
{
    public float Width = 0.5f;
        private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");

        public override Material materialForRendering {
            get {
                
                var forRendering = new Material(base.materialForRendering);
                forRendering.SetInt(StencilComp, (int)CompareFunction.NotEqual);
     
                return forRendering;
            }
        }
    }