using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KineticPreset: ICloneable
{
    public string PresetName;

    public List<FrequencyGap> Gaps = new List<FrequencyGap>();
    [NonSerialized]
    private List<Oscilator> Oscilators = new List<Oscilator>();
    public List<KineticPointInstance> Points = new List<KineticPointInstance>();
    public GenericFlag<int> MeshId = new GenericFlag<int>("MeshId", 0);

    public ModifyingParameter NearCutPlane;
    public ModifyingParameter FarCutPlane;
    public ModifyingParameter Lifetime;
    public ModifyingParameter ParticlesCount;
    public ModifyingParameter Gravity, Speed;

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

        PresetName = presetName;

        NearCutPlane = new ModifyingParameter(0f, 0f, 8f);
        FarCutPlane = new ModifyingParameter(5f, 0f, 8f);
        Lifetime = new ModifyingParameter(1f, 0f, 10f);
        ParticlesCount = new ModifyingParameter(100000, 1000, 1000000);
        Gravity = new ModifyingParameter(0f, -1f, 1f);
        Speed = new ModifyingParameter(0f, 0f, 1f);

        AddGap("Fire", 0.1f, 0.3f, Color.red, DefaultResources.GapSprites[1]);
        AddGap("Air", 0.3f, 0.3f, Color.cyan, DefaultResources.GapSprites[2]);
        AddGap("Earth", 0.6f, 0.3f, Color.green, DefaultResources.GapSprites[3]);
        AddGap("Water", 0.9f, 0.3f, Color.blue, DefaultResources.GapSprites[4]);

        Points.Add(new KineticPointInstance(0, "Настройки"));
        Points.Add(new KineticPointInstance(1, "Цвет с глубиной"));
        Points.Add(new KineticPointInstance(2, "Завихрения"));

        Points.Add(new KineticPointInstance(3, "Непонятные искажения"));
        Points.Add(new KineticPointInstance(4, "Шлейф с глубиной"));

        Points.Add(new KineticPointInstance(5, "Шлейф"));
        Points.Add(new KineticPointInstance(6, "Цвет"));
        Points.Add(new KineticPointInstance(7, "Размер с глубиной"));
        Points.Add(new KineticPointInstance(8, "Отталкивающая сила"));
        Points.Add(new KineticPointInstance(9, "Искажение простое"));
        Points.Add(new KineticPointInstance(10, "Цвет со временем"));
        Points.Add(new KineticPointInstance(11, "Размер"));
        Points.Add(new KineticPointInstance(12, "Зигзаг"));
    }


    public FrequencyGap AddGap(string name, float pos, float size, Color color, Sprite sprite)
    {
        FrequencyGap fg = new FrequencyGap(name, pos, size, color, sprite);
        Gaps.Add(fg);
        return fg;
    }

    public object Clone()
    {
        Debug.Log("CLONE");

        KineticPreset clone = new KineticPreset(PresetName);
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

        for (int i = 0;i< clone.Gaps.Count;i++)
        {
            clone.Gaps[i] = Gaps[i].Clone() as FrequencyGap;
            i++;
        }

        clone.Init();
        return clone;
    }
}