﻿using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvePickWindow : Singleton<CurvePickWindow>
{
    public GenericFlag<string> selectedCurveId = new GenericFlag<string>("selectedCurveId", "");

    public Action<string> OnPicked = (v) => { };
    private Action<string> callback;

    [SerializeField]
    private GameObject Btn;

    [SerializeField]
    private GameObject View;

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(PointChanged);
        CreateBtns();
        View.SetActive(false);
    }

    private void CreateBtns()
    {
        foreach (Transform t in View.transform)
        {
            if (t.gameObject.name != "AddBtn")
            {
                Destroy(t.gameObject);
            }
        }

        int i = 0;
        foreach (CurveInstance curve in SessionsManipulator.Instance.Curves.Curves)
        {
            StartCoroutine(CreateCurveBtn(curve, i * 0.05f));
            i++;
        }
    }

    private void PointChanged(KineticPoint obj)
    {
        Hide();   
    }

    private IEnumerator CreateCurveBtn(CurveInstance curve, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newGradientBtn = Instantiate(Btn);
        newGradientBtn.transform.SetParent(View.transform);
        newGradientBtn.GetComponent<GradientCurveBtn>().Set(curve);

    }

    public void SelectCurve(string id)
    {
        OnPicked(id);
        if (callback!=null)
        {
            callback(id);
        }
        Hide();
    }


    public void Show(string id, Action<string> callback = null)
    {
        selectedCurveId.SetState(id);
        KineticFieldController.Instance.KeysEnabled = false;
        this.callback = callback;
        View.SetActive(true);

       
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        KineticFieldController.Instance.KeysEnabled = true;
        View.SetActive(false);
    }

    public void AddCurve()
    {
        Debug.Log("AddCurve");
        AnimationCurve nc = new AnimationCurve(new Keyframe[] {new Keyframe(0f,0f), new Keyframe(1f,0f) });
        CurveInstance newCurve = new CurveInstance(nc);
        StartCoroutine(CreateCurveBtn(newCurve, 0.05f));
        selectedCurveId.SetState(newCurve.Id);
        SessionsManipulator.Instance.Curves.Curves.Add(newCurve);
        SessionsManipulator.Instance.SaveCurves();
    }
}
