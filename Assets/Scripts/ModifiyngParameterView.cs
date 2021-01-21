using System;
using System.Collections;
using com.armatur.common.flags;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModifiyngParameterView : MonoBehaviour
{
    public ModifyingParameter Parameter;
    private Coroutine dragCoroutine = null;

    [SerializeField]
    private int decRound = 2;

    [SerializeField]
    private TMPro.TextMeshProUGUI ValueText;
    [SerializeField]
    private Slider Slider;
    [SerializeField]
    private ArcSlider MultiplicatorSlider;
    [SerializeField]
    private RectTransform ExtraValue;
    [SerializeField]
    private GameObject AddSourceBtn;

    private void Start()
    {
        ExtraValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        MultiplicatorSlider.OnValueChanged += MultiplyerValueChanged;
    }

    private void MultiplyerValueChanged(float v)
    {
        Parameter.Multiplicator.SetState(v);
    }



    public void Init(ModifyingParameter parameter)
    {
        Slider.onValueChanged.RemoveAllListeners();

       
        if (Parameter!=null)
        {
            Parameter.SourceId.RemoveListener(SourceChanged);
            Parameter.Value.RemoveListener(ParameterValueChanged);
        }
        Parameter = parameter;
        if (Parameter!=null)
        {

            Slider.minValue = parameter.MIN;
            Slider.maxValue = parameter.MAX;
            Slider.value = parameter.BaseValue.Value;
            parameter.SourceId.AddListener(SourceChanged);
            parameter.Value.AddListener(ParameterValueChanged);
            ParameterValueChanged(parameter.Value.Value);
            SourceChanged(parameter.SourceId.Value);
            ValueText.text = Math.Round(parameter.BaseValue.Value, decRound).ToString();
        }

        Slider.onValueChanged.AddListener(BaseSliderChanged);
    }

    private void SourceChanged(int sourceId)
    {
        AddSourceBtn.SetActive(Parameter.Source == null);
        MultiplicatorSlider.gameObject.SetActive(Parameter.Source != null);
        if (Parameter.Source != null)
        {
            MultiplicatorSlider.Value = Parameter.Multiplicator.Value;
            MultiplicatorSlider.Init(Parameter.Source.Icon);
        }
    }

    private void ParameterValueChanged(float v)
    {     
        if (dragCoroutine==null)
        {
            ValueText.text = Math.Round(v, decRound).ToString();
            float unitsInValue = Slider.GetComponent<RectTransform>().rect.width / (Slider.maxValue - Slider.minValue);
            if (v > 0)
            {
                ExtraValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v * unitsInValue);
                ExtraValue.pivot = new Vector2(0, 0.5f);
                ExtraValue.GetComponent<Image>().color = Color.white;
            }
            else
            {
                ExtraValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -v * unitsInValue);
                ExtraValue.pivot = new Vector2(1, 0.5f);
                ExtraValue.GetComponent<Image>().color = Color.gray;
            }
        }
        else
        {
            ExtraValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        }
    }

    private void BaseSliderChanged(float v)
    {
        Debug.Log("1");
        Parameter.SliderValueChanged(v);
  
        ValueText.text = Math.Round(v, decRound).ToString();
        if (dragCoroutine!=null)
        {
            StopCoroutine(dragCoroutine);
            dragCoroutine = null;
        }

        dragCoroutine = StartCoroutine(DragC());
    }

    private IEnumerator DragC()
    {
        yield return new WaitForSeconds(0.5f);
        dragCoroutine = null;
    }

    public void ChooseSource()
    {

        int index = KineticFieldController.Instance.Sources.IndexOf(Parameter.Source);
        if (index == -1)
        {
            index = 0;
        }
        FindObjectOfType<SourcePickWindow>().Show(index, (v)=>
        {
            if (v == 0)
            {
                Parameter.SetSource(null);
            }
            else
            {
                Parameter.SetSource(KineticFieldController.Instance.Sources[v]);
            }
        });
    }
}