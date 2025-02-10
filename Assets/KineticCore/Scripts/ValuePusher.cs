using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuePusher : BaseSignalSource
{
    [SerializeField]
    private BaseSignalSource source;
   
    // Update is called once per frame
    void Update()
    {
        if (source==null)
        {
            Signal.Value += Time.deltaTime *  multiplyer + extraValue;
        }
        else
        {
            Signal.Value += Time.deltaTime * (source.MultipliedSignal.Value * multiplyer + extraValue);
        }
    }
}
