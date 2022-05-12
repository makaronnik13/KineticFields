using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class SourceView : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text Value, Name;

    [SerializeField]
    private Slider Slider;

    private Source source;

    public void Init(Source source)
    {
        this.source = source;
        Name.text = source.Name;
        source.Value.Subscribe((v)=>
        {
            Slider.minValue = source.minValue;
            Slider.maxValue = source.maxValue;
            Slider.value = (float)v;
            Value.text = ((float)v).ToString("F1");
        });
    }
}
