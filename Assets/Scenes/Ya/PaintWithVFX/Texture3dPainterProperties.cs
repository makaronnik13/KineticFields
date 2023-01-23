using UnityEngine;

    public sealed partial class Texture3dPainter : MonoBehaviour
    {
        #region Editor-only property
        [SerializeField] int _pointCount = 65536;

        void OnValidate()
        {
            _pointCount = Mathf.Max(64, _pointCount);

            // We assume that someone changed the values/references in the
            // serialized fields, so let us dispose the internal objects to
            // re-initialize them with the new values/references. #BADCODE
            DisposeInternals();
        }

        #endregion

        #region Hidden asset reference

        [SerializeField] ComputeShader _compute = null;

        #endregion

        #region Runtime-only properties

        public Texture PositionMap => _positionMap;
        public int VertexCount => _pointCount;

        #endregion
        
    }