using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidetValue : MonoBehaviour
{
    [SerializeField]
    private Slider Slider;

    [SerializeField]
    private int dec = 2;

    private TMPro.TextMeshProUGUI text;

   
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        Slider.onValueChanged.AddListener(ValueChanged);
    }

    private void ValueChanged(float v)
    {
        text.text = System.Math.Round(v, dec).ToString();
    }
}
