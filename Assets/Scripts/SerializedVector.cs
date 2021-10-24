using System;
using UnityEngine;

[Serializable]
public class SerializedVector 
{
    public float x, y, z;

    public Vector3 Vector3
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public SerializedVector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
