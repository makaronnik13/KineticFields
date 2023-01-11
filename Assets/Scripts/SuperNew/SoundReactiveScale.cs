using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReactiveScale : MonoBehaviour
{

    [SerializeField] private SignalSource signalSource;


    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * signalSource.MultipliedValue;
    }
}
