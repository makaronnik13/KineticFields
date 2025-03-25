using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientCurveBtn : MonoBehaviour
{
    private CurveInstance curve;
    private GradientInstance gradient;

    private void Start()
    {

    }

    private void UpdateFrame()
    {
        if (gradient != null)
        {
            transform.GetChild(0).gameObject.SetActive(GradientPickWindow.Instance.selectedGradientId.Value == gradient.Id);
        }
        else if(curve!= null)
        {
            transform.GetChild(0).gameObject.SetActive(CurvePickWindow.Instance.selectedCurveId.Value == curve.Id);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    public void Set(CurveInstance curve)
    {
        if (this.gradient != null)
        {
            this.gradient.OnEdited -= GradientEdited;
        }
        if (this.curve != null)
        {
            this.curve.OnEdited -= CurveEdited;
        }

        this.curve = curve;
        this.gradient = null;
        this.curve.OnEdited += CurveEdited;

        GetComponent<Image>().sprite = CurveEditor.Instance.MakeScreenshot(curve.Curve);
        GetComponent<Button>().onClick.RemoveAllListeners();

        GetComponent<Button>().onClick.AddListener(() => CurvePickWindow.Instance.SelectCurve(curve.Id));

    }

    public void Set(GradientInstance gradient)
    {
        if (this.gradient!=null)
        {
            this.gradient.OnEdited -= GradientEdited;
        }
        if (this.curve != null)
        {
            this.curve.OnEdited -= CurveEdited;
        }

        this.gradient = gradient;
        this.gradient.OnEdited += GradientEdited;
        this.curve = null;

        GetComponent<Image>().sprite = GradientPickWindow.Instance.GetImage(gradient.Gradient);
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => GradientPickWindow.Instance.SelectGradient(gradient.Id));

     
    }

    private void Update()
    {
        UpdateFrame();
    }

    private void GradientEdited()
    {
        GetComponent<Image>().sprite = GradientPickWindow.Instance.GetImage(gradient.Gradient);
    }

    private void CurveEdited()
    {
        Debug.Log("E2");

        GetComponent<Image>().sprite = CurveEditor.Instance.MakeScreenshot(curve.Curve);
    }

    public void Edit()
    {
        if (gradient!=null)
        {
            GradientEditor.Instance.Edit(gradient);
        }
        else
        {
            CurveRedactor.Instance.EditCurve(curve);
        }
    }
}
