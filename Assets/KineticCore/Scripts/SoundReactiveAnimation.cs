using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReactiveAnimation : MonoBehaviour
{
    [SerializeField] private SignalSource signalSource;
    [SerializeField] private Animator animator;
    [SerializeField] private float baseSpeed;
    
    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", baseSpeed+signalSource.MultipliedValue);
    }
}
