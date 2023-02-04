using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BaseSignalSource: MonoBehaviour
{
    [HideInInspector]
    public ReactiveProperty<float> Signal = new ReactiveProperty<float>();

    [HideInInspector]
    public ReactiveProperty<float> MultipliedSignal = new ReactiveProperty<float>();

    [SerializeField] public float multiplyer = 1;
    [SerializeField] public float extraValue = 1;

    void Start()
    {
        Signal.Subscribe(s =>
        {
            MultipliedSignal.Value = s * multiplyer + extraValue;
        }).AddTo(this);
    }
}
