using System;
using UniRx;
using UnityEngine;

public abstract class InputSO: ScriptableObject
{
    [HideInInspector]
    public int UniqueId;
    public ReactiveProperty<float> Value = new ReactiveProperty<float>();


    public virtual void SetValue(float value)
    {
        Value.Value = value;
    }

    private void OnValidate()
    {
        UniqueId = GetInstanceID();
    }

    public InputSO CreateInstance(InputSO so)
    {
        var copy = Instantiate(so);
        copy.UniqueId = so.UniqueId;
        return copy;
    }

}
