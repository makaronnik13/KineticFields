using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPointInspector : Singleton<MainPointInspector>
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    private VariantPicker Mesh;

    [SerializeField]
    private CurvePicker Curve;

    [SerializeField]
    private GradientPicker Gradient;

    [SerializeField]
    private ModifiyngParameterView Size, NearCutPlane, FarCutPlane, Count;
    [SerializeField]
    private ModifiyngParameterView LifetimeSlider;


    // Start is called before the first frame update
    void Start()
    {
        Curve.OnCurvePicked += CurveChanged;
        Mesh.OnIdChanged += MeshChanged;
        Gradient.OnGradientPicked += GradientChanged;
        KineticFieldController.Instance.ActivePoint.AddListener(ActivePointChanged);
    }

    public void PresetChanged(KineticPreset preset)
    {
        preset = KineticFieldController.Instance.Session.Value.ActivePreset.Value;


        Curve.SetValue(preset.MainPoint.CurveId);
        Gradient.SetValue(preset.MainPoint.gradientId);
        Mesh.SetValue(preset.MeshId.Value);
        Count.Init(preset.ParticlesCount);
        Size.Init(preset.MainPoint.Deep);
        NearCutPlane.Init(preset.NearCutPlane);
        FarCutPlane.Init(preset.FarCutPlane);
        LifetimeSlider.Init(preset.Lifetime);
       
    }

    private void GradientChanged(int gradient)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.gradientId = gradient;
        KineticFieldController.Instance.Visual.SetGradient("P0Gradient", DefaultResources.Settings.Gradients[gradient]);
    }

    private void MeshChanged(int v)
    {
        KineticFieldController.Instance.Session.Value.ActivePreset.Value.MeshId.SetState(v);
        KineticFieldController.Instance.Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[v]);
    }

    private void CurveChanged(int v)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.CurveId = v;
        KineticFieldController.Instance.Visual.SetAnimationCurve("P0Func", DefaultResources.Settings.SizeCurves[v]);
    }

    private void ActivePointChanged(KineticPoint point)
    {
        if (point != null && point.Point.Id == 0)
        {
            View.SetActive(true);



            KineticPreset  preset = KineticFieldController.Instance.Session.Value.ActivePreset.Value;
    

            Curve.SetValue(preset.MainPoint.CurveId);
            Gradient.SetValue(preset.MainPoint.gradientId);
            Mesh.SetValue(preset.MeshId.Value);
            Count.Init(preset.ParticlesCount);
            Size.Init(preset.MainPoint.Deep);
            NearCutPlane.Init(preset.NearCutPlane);
            FarCutPlane.Init(preset.FarCutPlane);
            LifetimeSlider.Init(preset.Lifetime);

        }
        else
        {
            View.SetActive(false);
        }

       
    }

    public void Hide()
    {
        View.SetActive(false);
    }

    public void TryShow()
    {
        if (!View.activeInHierarchy)
        {
            ActivePointChanged(KineticFieldController.Instance.ActivePoint.Value);
        }
    }
}
