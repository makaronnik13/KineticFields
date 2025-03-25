using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointInspector : MonoBehaviour
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    private CurvePicker Curve;

    [SerializeField]
    private GradientPick Gradient;

    [SerializeField]
    private ModifiyngParameterView Size, Deep, Speed, Volume;

    [SerializeField]
    private Toggle ActivationToggle;

    [SerializeField]
    private TMPro.TextMeshProUGUI PointName;

    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(ActivePointChanged);
        Curve.OnCurvePicked+= CurveChanged;
        Gradient.OnGradientPicked += GradientChanged;
        ActivationToggle.onValueChanged.AddListener(ActiveStateChanged);
    }

    private void GradientChanged(string v)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.Gradient = KineticFieldController.Instance.Session.Value.Gradients.GetGradient(v);// DefaultResources.Settings.Gradients[v];
    }

    private void CurveChanged(string v)
    {
        KineticFieldController.Instance.ActivePoint.Value.Point.Curve = KineticFieldController.Instance.Session.Value.Curves.GetCurve(v); //DefaultResources.Settings.SizeCurves[v];
    }

    private void ActiveStateChanged(bool v)
    {
        if (KineticFieldController.Instance.ActivePoint.Value!=null)
        {
            KineticFieldController.Instance.ActivePoint.Value.Point.Active.SetState(v);
        }
    }



    private void ActivePointChanged(KineticPoint point)
    {
        if (point == null || point.Point.Id == 0)
        {
            View.SetActive(false);
        }
        else
        {
            PointName.text = point.Point.Name;
            View.SetActive(true);
            Curve.SetValue(point.Point.Curve);
            Gradient.SetValue(point.Point.Gradient);
            Size.Init(point.Point.Radius);
            Deep.Init(point.Point.Deep);
            Volume.Init(point.Point.Volume);
            ActivationToggle.isOn = KineticFieldController.Instance.ActivePoint.Value.Point.Active.Value;
            Gradient.gameObject.SetActive(point.Point.ShowGradient);
        }
    }



    public void Hide()
    {
        View.SetActive(false);
    }

    public void TryShow()
    {
        ActivePointChanged(KineticFieldController.Instance.ActivePoint.Value);
    }
}
