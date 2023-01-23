using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

    [AddComponentMenu("VFX/Property Binders/Smrvfx/3d Painter Binder")]
    [VFXBinder("Smrvfx/3dPainter")]
    sealed class Texture3dPainterBinder : VFXBinderBase
    {
        public string PositionMapProperty {
            get => (string)_positionMapProperty;
            set => _positionMapProperty = value;
        }
        

        [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
        ExposedProperty _positionMapProperty = "PositionMap";

       
        public Texture3dPainter Target = null;

        public override bool IsValid(VisualEffect component)
            => Target != null &&
               component.HasTexture(_positionMapProperty);

        public override void UpdateBinding(VisualEffect component)
        {
            if (Target.PositionMap == null) return;

            component.SetTexture(_positionMapProperty, Target.PositionMap);
            
        }

        public override string ToString()
          => $"Painter 3d : '{_positionMapProperty}' -> {Target?.name ?? "(null)"}";
    }