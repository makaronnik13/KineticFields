using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BpmCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Image image;

    [Inject]
    public void Construct(ConstantBPMSource bpmSource)
    {
        if (text!=null)
        {
            bpmSource.OnBPMchanged.Subscribe(bpm =>
            {
                text.text = bpm + " bpm";
            }).AddTo(this);
        }

        if (image!=null)
        {
            bpmSource.OnBeat.Subscribe(_ =>
            {
                image.color = Color.white;
                Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
                {
                    image.color = Color.white * 0.2f;
                }).AddTo(this);
            }).AddTo(this);
        }
    }
}
