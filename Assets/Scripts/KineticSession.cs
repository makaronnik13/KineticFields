﻿
using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class KineticSession
{
    public List<FrequencyGap> Gaps = new List<FrequencyGap>();

    public List<Oscilator> Oscilators = new List<Oscilator>();

    public string SessionName;

    public GenericFlag<KineticPreset> ActivePreset = new GenericFlag<KineticPreset>("ActivePreset", null);

    public KineticPresetLine[] Lines = new KineticPresetLine[10]
    {
        new KineticPresetLine("Line_0"),
        new KineticPresetLine("Line_1"),
        new KineticPresetLine("Line_2"),
        new KineticPresetLine("Line_3"),
        new KineticPresetLine("Line_4"),
        new KineticPresetLine("Line_5"),
        new KineticPresetLine("Line_6"),
        new KineticPresetLine("Line_7"),
        new KineticPresetLine("Line_8"),
        new KineticPresetLine("Line_9")
    };

    public KineticSession()
    {
    }

    public void Init()
    {
        foreach (KineticPresetLine presetLine in Lines)
        {
            foreach (KineticPreset preset in presetLine.Presets)
            {
                preset.Init();
            }
        }
    }

    public KineticSession(string sessionName)
    {
        SessionName = sessionName;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Lines[i].Presets[j] = new KineticPreset("Preset_" + j);
            }
        }

        ActivePreset.SetState(Lines[0].Presets[0]);
 
        AddGap("Fire", 0.1f, 0.3f, Color.red, DefaultResources.GapSprites[1]);
        AddGap("Air", 0.3f, 0.3f, Color.cyan, DefaultResources.GapSprites[2]);
        AddGap("Earth", 0.6f, 0.3f, Color.green, DefaultResources.GapSprites[3]);
        AddGap("Water", 0.9f, 0.3f, Color.blue, DefaultResources.GapSprites[4]);

        AddOscilator(1, 0);
        AddOscilator(2, 0);
        AddOscilator(1, 1);
        AddOscilator(1, 1);
        AddOscilator(1, 2);
        AddOscilator(1, 3);
        AddOscilator(1, 4);
    }

    private void AddOscilator(float multiplyer, int repeatRate)
    {
        Oscilators.Add(new Oscilator(multiplyer, repeatRate));
    }

    public FrequencyGap AddGap(string name, float pos, float size, Color color, Sprite sprite)
        {
            FrequencyGap fg = new FrequencyGap(name, pos, size, color, sprite);
            Gaps.Add(fg);
            return fg;
        }

        public void LoadPreset(int i)
        {

        if (ActivePreset.Value!=null)
        {
            ActivePreset.Value.NearCutPlane.Value.RemoveListener(NearCutPlaneChanged);
            ActivePreset.Value.FarCutPlane.Value.RemoveListener(FarCutPlaneChanged);
            ActivePreset.Value.Lifetime.Value.RemoveListener(LifetimeChanged);
            ActivePreset.Value.ParticlesCount.Value.RemoveListener(PCountChanged);
            ActivePreset.Value.MainPoint.Deep.Value.RemoveListener(GlobalSizeChanged);
            ActivePreset.Value.MainPoint.Radius.Value.RemoveListener(NoizeChanged);
            ActivePreset.Value.MeshId.RemoveListener(MeshChanged);
        }

        ActivePreset.SetState(Lines[0].Presets[i]);
        ActivePreset.Value.Init();

        ActivePreset.Value.NearCutPlane.Value.AddListener(NearCutPlaneChanged);
        ActivePreset.Value.FarCutPlane.Value.AddListener(FarCutPlaneChanged);
        ActivePreset.Value.Lifetime.Value.AddListener(LifetimeChanged);
        ActivePreset.Value.ParticlesCount.Value.AddListener(PCountChanged);
       
        ActivePreset.Value.MainPoint.Deep.Value.AddListener(GlobalSizeChanged);
        ActivePreset.Value.MainPoint.Radius.Value.AddListener(NoizeChanged);
        ActivePreset.Value.MeshId.AddListener(MeshChanged);


        MainPointInspector.Instance.PresetChanged(ActivePreset.Value);
    }

    private void MeshChanged(int meshId)
    {
        KineticFieldController.Instance.Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[meshId]);
    }

    private void NoizeChanged(float val)
    {
        KineticFieldController.Instance.Visual.SetFloat("Noize", val);
    }

    private void GlobalSizeChanged(float val)
    {
        KineticFieldController.Instance.Visual.SetFloat("Size", (0.05f + val - 1f) / 8f);
    }

    private void PCountChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetInt("Rate", Mathf.RoundToInt(v));
    }

    private void LifetimeChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("Lifetime", v);
        //KineticFieldController.Instance.Visual.SetInt("Rate", DefaultResources.Settings.GetCount(v));
    }

    private void NearCutPlaneChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("FrontCutPlane", v);
    }

    private void FarCutPlaneChanged(float v)
    {
        KineticFieldController.Instance.Visual.SetFloat("BackCutPlane", v);
    }

    public void SavePreset(int i)
    {
        Debug.Log("save preset "+i);
        Lines[0].Presets[i] = ActivePreset.Value.Clone() as KineticPreset;
    }

  
}