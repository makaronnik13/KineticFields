using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SourcePickWindow : MonoBehaviour
{
    public Action<int> OnPicked = (v) => { };

    [SerializeField]
    private GameObject SourceBtn;

    [SerializeField]
    private GameObject View;

    private Action<int> callback;

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(PointChanged);
        int i = 0;
        foreach (ISource source in KineticFieldController.Instance.Sources)
        {
            CreateGradientBtn(source, i);
            i++;
        }
        View.SetActive(false);
    }

    private void PointChanged(KineticPoint obj)
    {
        Hide();
    }

    private void CreateGradientBtn(ISource source, int id)
    {
        GameObject newGradientBtn = Instantiate(SourceBtn);
        newGradientBtn.transform.SetParent(View.transform);
        newGradientBtn.GetComponent<Image>().sprite = source.Icon;
        newGradientBtn.GetComponent<Button>().onClick.AddListener(() => SelectSource(id));
    }

    private void SelectSource(int id)
    {
        if (callback!=null)
        {
            callback.Invoke(id);
        }
        OnPicked(id);
        Hide();
    }

    [ContextMenu("Show")]
    public void Test()
    {
        Show(3);
    }

    public void Show(int id, Action<int> onPick = null)
    {
        callback = onPick;
        View.SetActive(true);
        foreach (Transform t in View.transform)
        {
            t.GetChild(0).gameObject.SetActive(false);
        }
        if (id!=-1)
        {
            View.transform.GetChild(id).GetChild(0).gameObject.SetActive(true);
        }
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        View.SetActive(false);
    }
}
