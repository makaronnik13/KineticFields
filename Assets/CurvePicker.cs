using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvePicker : MonoBehaviour
{
    public Action<int> OnCurvePicked = (v) => {};

    private int currentId;

    [SerializeField]
    private Image Img;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=>
        {
            FindObjectOfType<CurvePickWindow>().Show(currentId, (v)=>
            {
                OnCurvePicked(v);
                SetValue(v);
            });
        });
    }

    public void SetValue(int curveId)
    {
        currentId = curveId;
        Img.sprite = CurveEditor.Instance.MakeScreenshot(DefaultResources.Settings.SizeCurves[curveId]);
    }
}
