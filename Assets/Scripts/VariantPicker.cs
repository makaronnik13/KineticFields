using System;
using UnityEngine;
using UnityEngine.UI;

public class VariantPicker : MonoBehaviour
{
    public Action<int> OnIdChanged = (v) => { };

    private void Start()
    {
        int i = 0;
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            int v = i;
            b.onClick.AddListener(() => { Pick(v); });
            i++;
        }
    }


    public void Pick(int curveId)
    {
        OnIdChanged(curveId);
        SetValue(curveId);
    }

    public void SetValue(int curveId)
    {
        int i = 0;
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.transform.GetChild(1).GetComponent<Image>().enabled = i == curveId;
            i++;
        }
    }
}