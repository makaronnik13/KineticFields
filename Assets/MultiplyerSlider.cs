using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplyerSlider : MonoBehaviour
{
    [SerializeField]
    private Slider Slider;

    [SerializeField]
    private BarSpectrum SpectrumBar;

    [SerializeField]
    private TMPro.TextMeshProUGUI Text;

    // Start is called before the first frame update
    void Start()
    {
        Slider.onValueChanged.AddListener(SliderValueChanged);
    }

    private void SliderValueChanged(float v)
    {
        SpectrumBar.AudioScale = v;
        Text.text = Mathf.RoundToInt(((v-Slider.minValue)/(Slider.maxValue-Slider.minValue))*100).ToString();
    }

}
