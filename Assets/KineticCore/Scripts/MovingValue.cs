using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingValue : BaseSignalSource
{

    [SerializeField]
    private SignalSource source;

    private void Update()
    {
        Signal.Value += Time.deltaTime * multiplyer * source.MultipliedValue;
    }
}
