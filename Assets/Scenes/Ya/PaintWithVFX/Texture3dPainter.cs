using System;
using System.Collections.Generic;
using UnityEngine;
using  Smrvfx;
using Unity.Collections;

[ExecuteInEditMode]
public sealed partial class Texture3dPainter : MonoBehaviour
{

    #region Internal objects

    ComputeBuffer _samplePoints;
    ComputeBuffer _positionBuffer1;

    RenderTexture _positionMap;

    private int index = 0;
    //private NativeArray<Vector3> positions;
    private List<Vector3> positions = new List<Vector3>();
    #endregion

    #region MonoBehaviour implementation

    void OnDisable()
      => DisposeInternals();

    void OnDestroy()
      => DisposeInternals();

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Paint( transform.InverseTransformPoint(
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f))));
        }
    }

    public void Paint(Vector3 pos)
    {
        pos = pos + Vector3.one * 10;
        pos.z = 0;
        
        pos*=0.1f;
        Debug.Log(pos);
        // Lazy initialization
        if (_samplePoints == null || _positionBuffer1 == null) InitializeInternals();

        index++;
        if (positions.Count<_pointCount)
        {
            positions.Add(pos);
        }
        else
        {
            if (index>=positions.Count)
            {
                index = 0;
            }
            
            positions[index] = pos;
        }
        
        
        using (var posi = MemoryUtil.TempJobArray<Vector3>(_pointCount))
        {
            for (int i = 0; i < _pointCount; i++)
            {
                if (i>=positions.Count)
                {
                    positions.Add(Vector3.zero);
                }
                posi[i].Set(positions[i].x, positions[i].y, positions[i].z);
            }
            _positionBuffer1.SetData(posi, 0, 0, positions.Count);
        }
        
        
        
        // ComputeBuffer -> RenderTexture
        TransferData();
    }

    #endregion

    #region Private methods

    void InitializeInternals()
    {
        // Sample point generation
        //positions = MemoryUtil.Array<Vector3>(_pointCount);


        NativeArray<SamplePoint> points = MemoryUtil.Array<SamplePoint>(_pointCount);
        _samplePoints = new ComputeBuffer(_pointCount, SamplePoint.SizeInByte);
        _samplePoints.SetData(points);

        // Intermediate buffer allocation
        var vcount = points.Length;
        var float3size = sizeof(float) * 3;
        _positionBuffer1 = new ComputeBuffer(vcount, float3size);
        // Destination render texture allocation
        var width = 256;
        var height = (((_pointCount + width - 1) / width + 7) / 8) * 8;
        _positionMap = RenderTextureUtil.AllocateFloat(width, height);
        points.Dispose();
    }

    void DisposeInternals()
    {
        _samplePoints?.Dispose();
        _samplePoints = null;

        _positionBuffer1?.Dispose();
        _positionBuffer1 = null;
        
        ObjectUtil.Destroy(_positionMap);
        _positionMap = null;
    }
    

    void TransferData()
    {
        _compute.SetInt("SampleCount", _pointCount);
        _compute.SetMatrix("Transform", transform.localToWorldMatrix);
        _compute.SetMatrix("OldTransform", transform.localToWorldMatrix);
        _compute.SetFloat("FrameRate", 1 / Time.deltaTime);

        _compute.SetBuffer(0, "SamplePoints", _samplePoints);
        _compute.SetBuffer(0, "PositionBuffer", _positionBuffer1);
        _compute.SetTexture(0, "PositionMap", _positionMap);

        var width = _positionMap.width;
        var height = _positionMap.height;
        _compute.Dispatch(0, width / 8, height / 8, 1);
    }

    #endregion
}
