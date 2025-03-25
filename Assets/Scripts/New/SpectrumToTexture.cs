using System.Collections.Generic;
using KineticFields;
using UniRx;
using Unity.Collections;
using UnityEngine;
using Zenject;

//
    // Spectrum texture baking utility
    //
    public sealed class SpectrumToTexture : MonoBehaviour
    {
        #region Material override struct

        [System.Serializable]
        public struct MaterialOverride
        {
            [SerializeField] Renderer _renderer;
            [SerializeField] string _propertyName;

            public Renderer Renderer {
                get => _renderer;
                set => _renderer = value;
            }

            public string PropertyName {
                get => _propertyName;
                set => _propertyName = value;
            }

            public int PropertyID => Shader.PropertyToID(_propertyName);
        }

        #endregion

        #region Editable attributes

        // Bake target render texture
        [SerializeField] RenderTexture _renderTexture = null;
        public RenderTexture renderTexture {
            get => _renderTexture;
            set => _renderTexture = value;
        }

        // Material override list
        [SerializeField] MaterialOverride[] _overrideList = null;
        public MaterialOverride[] overrideList {
            get => _overrideList;
            set => _overrideList = value;
        }

        #endregion

        #region Runtime public property

        // Baked spectrum texture
        public Texture texture => _texture;

        #endregion

        #region Private members
        
        Texture2D _texture;
        MaterialPropertyBlock _block;
        private FFTService fftService;
        #endregion

        #region MonoBehaviour implementation

        [Inject]
        public void Construct(FFTService fftService)
        {
            this.fftService = fftService;
            Observable.EveryUpdate().Subscribe(_=>UpdateTexture()).AddTo(this);
        }
        
        void OnDestroy()
        {
            if (_texture != null) Destroy(_texture);
        }

        void UpdateTexture()
        {
            float[] spectrum = fftService.GetSpectrum(0);
        if (spectrum.Length == 0)
        {
            return;
        }
            // Refresh the temporary texture when the resolution was changed.
            if (_texture != null && _texture.width != spectrum.Length)
            {
                Destroy(_texture);
                _texture = null;
            }

            // Lazy initialization of the temporary texture
            if (_texture == null)
                _texture = new Texture2D(spectrum.Length, 1,
                                         TextureFormat.RFloat, false)
                           { wrapMode = TextureWrapMode.Clamp };

            // Texture update
         
            _texture.LoadRawTextureData(new NativeArray<float>(spectrum, Allocator.Temp));
            _texture.Apply();

            // Update the external render texture.
            if (_renderTexture != null)
                Graphics.CopyTexture(_texture, _renderTexture);

            // Lazy initialization of the material property block.
            if (_block == null) _block = new MaterialPropertyBlock();

            // Apply the material overrides.
            foreach (var o in _overrideList)
            {
                o.Renderer.GetPropertyBlock(_block);
                _block.SetTexture(o.PropertyID, _texture);
                o.Renderer.SetPropertyBlock(_block);
            }
        }

        #endregion
    }
