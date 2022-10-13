using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPosition : MonoBehaviour
{
    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private Transform Target;

    private void OnAnimatorIK(int layerIndex)
    {
        Animator.bodyPosition = Target.position;
        Animator.bodyRotation = Target.rotation;
    }
}
