using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscSource : BaseSignalSource
{
    private void SetFloat(float v)
    {
        Signal.Value = v;
    }
}
