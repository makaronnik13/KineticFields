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
    private GradientPick Gradient;

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


        Curve.SetValue(preset.MainPoint.Curve);
        Gradient.SetValue(preset.MainPoint.Gradient);
        Mesh.SetValue(preset.MeshId.Value);
        Count.Init(preset.ParticlesCount);
        Size.Init(preset.MainPoint.Deep);
        NearCutPlane.Init(preset.NearCutPlane);
        FarCutPlane.Init(preset.FarCutPlane);
        LifetimeSlider.Init(preset.Lifetime);
       
    }

    private void GradientChanged(string gradientId)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.Gradient = SessionsManipulator.Instance.Gradients.GetGradient(gradientId);
        KineticFieldController.Instance.Visual.SetGradient("P0Gradient", KineticFieldController.Instance.ActivePoint.Value.Point.Gradient.Gradient);
    }

    private void MeshChanged(int v)
    {
        KineticFieldController.Instance.Session.Value.ActivePreset.Value.MeshId.SetState(v);
        KineticFieldController.Instance.Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[v]);
    }

    private void CurveChanged(string curveId)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.Curve = SessionsManipulator.Instance.Curves.GetCurve(curveId);
        KineticFieldController.Instance.Visual.SetAnimationCurve("P0Func", KineticFieldController.Instance.ActivePoint.Value.Point.Curve.Curve);
    }

    private void ActivePointChanged(KineticPoint point)
    {
        if (point != null && point.Point.Id == 0)
        {
            View.SetActive(true);



            KineticPreset  preset = KineticFieldController.Instance.Session.Value.ActivePreset.Value;
    

            Curve.SetValue(preset.MainPoint.Curve);
            Gradient.SetValue(preset.MainPoint.Gradient);
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
