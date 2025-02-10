using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalRotator : MonoBehaviour
{
    [SerializeField] BaseSignalSource signal;
    [SerializeField] Vector3 vector;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(vector*signal.MultipliedSignal.Value);     
    }
}
