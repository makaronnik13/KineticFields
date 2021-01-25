using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveRedactor : Singleton<CurveRedactor>
{
    public AnimationCurve Curve;
    private AnimationCurve editingCurve;
    private int samples = 50;
    private float horizontalMultiplyer = 0.5f;

    public GameObject HandlePrefab;

    public GenericFlag<CurveHandle> EditingPoint = new GenericFlag<CurveHandle>("EditingPoint", null);

    [SerializeField]
    private LineRenderer Line3d;
    public GameObject View;

    public void Apply()
    {
        Curve = new AnimationCurve(editingCurve.keys);
    }

    [ContextMenu("test")]
    public void Test()
    {
        EditCurve(Curve);
    }

    public void EditCurve(AnimationCurve curve)
    {
        Curve = curve;
        editingCurve = new AnimationCurve(Curve.keys);
        View.SetActive(true);
        UpdateCurve();
    }


    private void UpdateCurve()
    {
        Line3d.positionCount = samples;

        for (int i = 0; i < samples; i++)
        {
            float hv = (i + 0.0f) / samples;
            Line3d.SetPosition(i, new Vector3(hv/horizontalMultiplyer, editingCurve.Evaluate(hv),0));
        }
    }
}
