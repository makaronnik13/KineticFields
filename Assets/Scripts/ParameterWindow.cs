using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterWindow : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Value;

    [SerializeField]
    private Slider ValueSlider;

    [SerializeField]
    private TMPro.TMP_Dropdown GapType;

    [SerializeField]
    private TMPro.TMP_InputField BaseValue, Multiplyer;


    [SerializeField]
    private TMPro.TextMeshProUGUI ParamName;

    private FrequencyGap Gap
    {
        get
        {
            return FrequencyGap.Bass; //FindObjectOfType<KineticFieldController>().ActiveGap.Value;
        }
    }

    void Start()
    {
       // FindObjectOfType<KineticFieldController>().ActiveGap.AddListener(ActiveGapChanged);
        GapType.onValueChanged.AddListener(GapTypeChanged);
        Multiplyer.onValueChanged.AddListener(MultiplyerChanged);
        BaseValue.onValueChanged.AddListener(BaseValueChanged);
    }

    private void BaseValueChanged(string value)
    {
        //Gap.BaseValue.SetState(float.Parse(value));
    }

    private void MultiplyerChanged(string value)
    {
        //Gap.Multiplyer.SetState(float.Parse(value));
    }

    private void GapTypeChanged(int v)
    {
        //FrequencyGap.GapType gt = (FrequencyGap.GapType)v;
        //Gap.GapT = gt;
        //Multiplyer.gameObject.SetActive(gt == FrequencyGap.GapType.Music);
    }

    private void ActiveGapChanged(FrequencyGap gap)
    {
        if (Gap != null)
        {
            //GapType.value = (int)gap.GapT;
            //BaseValue.text = gap.BaseValue.Value.ToString();
            //Multiplyer.text = gap.Multiplyer.Value.ToString();
            //ParamName.text = gap.Name;
        }
    }

    private void Update()
    {
        if (Gap != null)
        {
            //Value.text = System.Math.Round(Gap.Value,2).ToString();
            //ValueSlider.value = Gap.Value / 100f;
        }
    }

}
