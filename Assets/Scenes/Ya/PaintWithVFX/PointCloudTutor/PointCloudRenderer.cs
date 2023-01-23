using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PointCloudRenderer : MonoBehaviour
{
    [SerializeField] private MeshFilter brush3d;
    
    
    Texture2D texColor;
    Texture2D texPosScale;
    public VisualEffect vfx;
    public uint resolution = 2048;

    public float particleSize = 0.1f;
    
    bool toUpdate = false;
    uint particleCount = 0;

    
    Vector3[] vector3 = new Vector3[0];
    Color[] colors = new Color[0];
    private int id = 0;

    [SerializeField] private float paintSpeed;

    private CompositeDisposable process = new CompositeDisposable();

    private void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
            {
                process.Clear();
            
                Vector3[] vertices = brush3d.mesh.vertices;
                vertices = vertices.OrderBy(v => v.magnitude).ToArray();
                int id = 0;
                
                Observable.Interval(TimeSpan.FromSeconds(paintSpeed)).Subscribe(_ =>
                {
                    if (id >= vertices.Length-1)
                    {
                        id = 0;
                    }
                    else
                    {
                        id++;
                        Matrix4x4 localToWorld = brush3d.transform.localToWorldMatrix;
                        Paint(localToWorld.MultiplyPoint3x4(vertices[id]));
                    }
                }).AddTo(process);
            }).AddTo(this);
        
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0)).Subscribe(_ =>
            {
                process.Clear();
            }).AddTo(this);
        



            //vfx = GetComponent<VisualEffect>();

            vector3 = new Vector3[resolution * resolution];
            colors = new Color[resolution * resolution];

            for (int i = 0; i < resolution * resolution; i++)
            {
                vector3[i] = Vector3.zero; //new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
                colors[i] = new Color(Random.value, Random.value, Random.value);
            }
            
    }

    private void Update() 
    {
        if (toUpdate) 
        {
            toUpdate = false;

            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("ParticleCount"), particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), texPosScale);
            vfx.SetUInt(Shader.PropertyToID("Resolution"), resolution);
        }
        SetParticles(vector3, colors);
    }

    private void Paint(Vector3 point)
    {
       
        id++;
        if (id>=resolution*resolution)
        {
            id = 0;
        }
        vector3[id] = point;
    }

    public void SetParticles(Vector3[] positions, Color[] colors) {
        texColor = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        int texWidth = texColor.width;
        int texHeight = texColor.height;

        for (int y = 0; y < texHeight; y++) {
            for (int x = 0; x < texWidth; x++) {
                int index = x + y * texWidth;
                texColor.SetPixel(x, y, colors[index]);
                var data = new Color(positions[index].x, positions[index].y, positions[index].z, particleSize);
                texPosScale.SetPixel(x, y, data);
            }
        }

        texColor.Apply();
        texPosScale.Apply();
        particleCount = (uint)positions.Length;
        toUpdate = true;
    }
}
