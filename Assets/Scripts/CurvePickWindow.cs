using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvePickWindow : MonoBehaviour
{
    public Action<int> OnPicked = (v) => { };
    private Action<int> callback;

    [SerializeField]
    private GameObject Btn;

    [SerializeField]
    private GameObject View;

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(PointChanged);
        int i = 0;
        foreach (AnimationCurve curve in DefaultResources.Settings.SizeCurves)
        {
            StartCoroutine(CreateCurveBtn(curve, i, i*0.2f));
            i++;
        }
        View.SetActive(false);
    }

    private void PointChanged(KineticPoint obj)
    {
        Hide();   
    }

    private IEnumerator CreateCurveBtn(AnimationCurve curve, int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newGradientBtn = Instantiate(Btn);
        newGradientBtn.transform.SetParent(View.transform);
        newGradientBtn.GetComponent<Image>().sprite = CurveEditor.Instance.MakeScreenshot(curve);
        newGradientBtn.GetComponent<Button>().onClick.AddListener(() => SelectCurve(id));
    }

    private void SelectCurve(int id)
    {
        OnPicked(id);
        if (callback!=null)
        {
            callback(id);
        }
        Hide();
    }

    [ContextMenu("Show")]
    public void Test()
    {
        Show(3);
    }

    public void Show(int id, Action<int> callback = null)
    {
        Debug.Log(id);
        this.callback = callback;
        View.SetActive(true);
        foreach (Transform t in View.transform)
        {
            t.GetChild(0).gameObject.SetActive(false);
        }
        View.transform.GetChild(id).GetChild(0).gameObject.SetActive(true);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        View.SetActive(false);
    }
}
