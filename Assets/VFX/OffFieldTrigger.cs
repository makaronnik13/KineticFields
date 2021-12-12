using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OffFieldTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PongBall>())
        {
            other.GetComponent<PongBall>().Crash();
            GetComponentInChildren<VisualEffect>().SendEvent("Spawn");
        }
    }
}
