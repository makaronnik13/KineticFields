using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuePusher : BaseSignalSource
{
    [SerializeField]
    private SignalSource source;
   
    // Update is called once per frame
    void Update()
    {
        Signal.Value += Time.deltaTime * (source.MultipliedValue * multiplyer + extraValue);
    }
}
