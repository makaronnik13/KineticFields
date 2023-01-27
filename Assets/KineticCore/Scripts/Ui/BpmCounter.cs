using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class BpmCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [Inject]
    public void Construct(ConstantBPMSource bpmSource)
    {
        bpmSource.OnBPMchanged.Subscribe(bpm =>
        {
            text.text = bpm + " bpm";
        }).AddTo(this);
    }
}
