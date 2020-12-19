
using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KineticSession
{
    public string SessionName;

    public GenericFlag<KineticPreset> ActivePreset = new GenericFlag<KineticPreset>("ActivePreset", null);

    public KineticPreset[] Presets = new KineticPreset[10];

    public KineticSession()
    {
    }

    public void Init()
    {
        foreach (KineticPreset preset in Presets)
        {
            preset.Init();
        }
    }

    public KineticSession(string sessionName)
    {
        SessionName = sessionName;
        for (int i = 0; i < 10; i++)
        {
            Presets[i] = new KineticPreset("Preset_"+i);
        }
        Debug.Log("LOAD 0");
        ActivePreset.SetState(Presets[0]);
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

        ActivePreset.SetState(Presets[i]);
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
        Presets[i] = ActivePreset.Value.Clone() as KineticPreset;
    }
}