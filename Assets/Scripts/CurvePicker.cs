using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvePicker : MonoBehaviour
{
    public Action<string> OnCurvePicked = (v) => {};

    private string currentId;

    [SerializeField]
    private Image Img;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=>
        {
            FindObjectOfType<CurvePickWindow>().Show(currentId, (v)=>
            {
                OnCurvePicked(v);
                SetValue(KineticFieldController.Instance.Session.Value.Curves.GetCurve(v));
            });
        });
    }

    public void SetValue(CurveInstance curve)
    {
        currentId = curve.Id;
        Img.sprite = CurveEditor.Instance.MakeScreenshot(curve.Curve);
    }
}
