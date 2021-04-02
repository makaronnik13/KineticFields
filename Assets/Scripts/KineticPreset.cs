using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KineticPreset: ICloneable
{
    public string PresetName;

    public Vector2 Position
    {
        get
        {
            return new Vector2(X,Y);
        }
        set
        {
            X = value.x;
            Y = value.y;
            OnPositionChanged(Position);
        }
    }

    [NonSerialized]
    public Action<Vector2> OnPositionChanged = (v) => { };

    public float X, Y;

    public List<KineticPointInstance> Points = new List<KineticPointInstance>();
    public GenericFlag<int> MeshId = new GenericFlag<int>("MeshId", 0);

    public ModifyingParameter NearCutPlane;
    public ModifyingParameter FarCutPlane;
    public ModifyingParameter Lifetime;
    public ModifyingParameter ParticlesCount;
    public ModifyingParameter Gravity, Speed;

    public GenericFlag<int> IconId = new GenericFlag<int>("IconId", 0);
    public GenericFlag<string> color = new GenericFlag<string>("Color","00FF00FF");
    public GenericFlag<string> color2 = new GenericFlag<string>("Color", "FF0000FF");

    public Sprite Icon
    {
        get
        {
            return DefaultResources.PresetSprites[IconId.Value];
        }
        set
        {
            IconId.SetState(DefaultResources.PresetSprites.IndexOf(value));
        }
    }

    public Color Color
    {
        get
        {
            Color c = Color.white;
            ColorUtility.TryParseHtmlString("#"+color.Value, out c);
            return c;
        }
        set
        {
            color.SetState(ColorUtility.ToHtmlStringRGBA(value));
        }
    }
    public Color Color2
    {
        get
        {
            Color c = Color.white;
            ColorUtility.TryParseHtmlString("#" + color2.Value, out c);
            return c;
        }
        set
        {
            color2.SetState(ColorUtility.ToHtmlStringRGBA(value));
        }
    }

    public string Id;

    public KineticPointInstance MainPoint
    {
        get
        {
            return Points.FirstOrDefault(p => p.Id == 0);
        }
    }

    public void Init()
    {
        NearCutPlane.Init();
        FarCutPlane.Init();
        Lifetime.Init();
        ParticlesCount.Init();
        Gravity.Init();
        Speed.Init();
        foreach (KineticPointInstance point in Points)
        {
            point.Init();
        }
    }

    public KineticPreset()
    {
     
    }

    public KineticPreset(string presetName)
    {
        Id = Guid.NewGuid().ToString();

        IconId.SetState(UnityEngine.Random.Range(0, DefaultResources.PresetSprites.Count - 1));
        Color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.3f, 0.3f, 1f, 1f);
        Color2 = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);

        PresetName = presetName;

        NearCutPlane = new ModifyingParameter(0f, 0f, 8f);
        FarCutPlane = new ModifyingParameter(5f, 0f, 8f);
        Lifetime = new ModifyingParameter(1f, 0f, 10f);
        ParticlesCount = new ModifyingParameter(100000, 100, 500000);
        Gravity = new ModifyingParameter(0f, -1f, 1f);
        Speed = new ModifyingParameter(0f, 0f, 1f);



        Points.Add(new KineticPointInstance(0, "Настройки", Vector3.zero));

        Points.Add(new KineticPointInstance(1, "Цвет с глубиной", new Vector3(-0.25f, 0.5f, 0)));
        Points.Add(new KineticPointInstance(6, "Цвет", new Vector3(0f, 0.5f, 0)));
        Points.Add(new KineticPointInstance(10, "Цвет со временем", new Vector3(0.25f, 0.5f, 0)));


        Points.Add(new KineticPointInstance(5, "Шлейф", new Vector3(-0.5f, 0.1f, 0)));
        Points.Add(new KineticPointInstance(4, "Шлейф с глубиной", new Vector3(-0.5f, -0.1f, 0)));

        Points.Add(new KineticPointInstance(7, "Размер с глубиной", new Vector3(0.5f, 0.1f, 0)));
        Points.Add(new KineticPointInstance(11, "Размер", new Vector3(0.5f, -0.1f, 0)));

        Points.Add(new KineticPointInstance(2, "Завихрения", new Vector3(-0.2f, 0.2f, 0)));
        Points.Add(new KineticPointInstance(12, "Бесполезная точка", new Vector3(0.2f, 0.2f, 0)));

        Points.Add(new KineticPointInstance(3, "Непонятные искажения", new Vector3(-0.25f, -0.5f, 0)));
        Points.Add(new KineticPointInstance(9, "Искажение простое", new Vector3(0f, -0.5f, 0)));
        Points.Add(new KineticPointInstance(8, "Отталкивающая сила", new Vector3(0.25f, -0.5f, 0)));   
    }

     

    public object Clone()
    {
        Debug.Log("CLONE");

        KineticPreset clone = new KineticPreset(PresetName);
        clone.Id = System.Guid.NewGuid().ToString();
        clone.IconId.SetState(IconId.Value);
        clone.Color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.3f, 0.3f, 1f, 1f);
        clone.Color2 = Color2;
        clone.FarCutPlane = FarCutPlane.Clone() as ModifyingParameter;
        clone.Gravity = Gravity.Clone() as ModifyingParameter;
        clone.Lifetime = Lifetime.Clone() as ModifyingParameter;
        clone.MeshId.SetState(MeshId.Value);
        clone.NearCutPlane = NearCutPlane.Clone() as ModifyingParameter;
        clone.ParticlesCount = ParticlesCount.Clone() as ModifyingParameter;
        clone.Speed = Speed.Clone() as ModifyingParameter;
      
        for (int i = 0; i < clone.Points.Count; i++)
        {
            clone.Points[i] = Points[i].Clone() as KineticPointInstance;
        }


        clone.Init();
        return clone;
    }
}