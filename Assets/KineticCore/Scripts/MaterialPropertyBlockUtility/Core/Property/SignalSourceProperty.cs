using System;
using UnityEngine;

[Serializable]
public class SignalSourceProperty : Property
{
    [SerializeField] private BaseSignalSource source;
    public override void Set(MaterialPropertyBlock prop)
    {
        if (source == null)
        {
            prop.SetFloat(ReferenceName, 0);
            return;
        }
        prop.SetFloat(ReferenceName, source.MultipliedSignal.Value);
    }
}