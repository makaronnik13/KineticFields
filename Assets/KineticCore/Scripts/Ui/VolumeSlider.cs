using KinectVfx;
using KineticFields;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text value;

    private FFTService fftService;

    [Inject]
    public void Construct(FFTService fftService)
    {
        this.fftService = fftService;
        if (fftService == null)
        {
            Debug.LogWarning("No fft service");
            return;
        }
        //slider.value = kinectPointCloud;
        value.text = fftService.multiplyer.ToString();

        slider.onValueChanged.AsObservable().Subscribe(v =>
        {
            fftService.multiplyer = v;
            value.text = String.Format("{0:0.#}", v);
        }).AddTo(this);
    }

    public void SetValueOsc(float v)
    {
        slider.value = slider.maxValue * v;
    }
}
