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
        MultiplicatorSlider.transform.GetChild(0).gameObject.SetActive(Parameter.Source != null);
        MultiplicatorSlider.transform.GetChild(1).gameObject.SetActive(Parameter.Source == null);

        if (Parameter.Source != null)
        {
            MultiplicatorSlider.Value = Parameter.Multiplicator.Value;
            MultiplicatorSlider.Init(Parameter.Source.Sprite);
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



    public void ChooseSource(Source source)
    {
        Parameter.SetSource(source);
    }
}