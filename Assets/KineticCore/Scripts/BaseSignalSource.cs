using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BaseSignalSource: MonoBehaviour
{
    public ReactiveProperty<float> Signal = new ReactiveProperty<float>();
}
