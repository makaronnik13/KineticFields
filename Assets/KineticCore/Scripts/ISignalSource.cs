using UniRx;
using UnityEngine;

public interface ISignalSource 
{
    ReactiveProperty<float> OutputSignal { get; }
}
