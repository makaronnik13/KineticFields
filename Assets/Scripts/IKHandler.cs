using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour
{
    [SerializeField]
    private Transform Target;

    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private AvatarIKGoal Hint;

    [Range(0f,1f)]
    [SerializeField]
    private float Weight;

    //[SerializeField]
    //private 

    private void OnAnimatorIK(int layerIndex)
    {
        Animator.SetIKPosition(Hint, Target.transform.position);
        Animator.SetIKRotation(Hint, Target.transform.rotation);
        Animator.SetIKPositionWeight(Hint, Weight);
        Animator.SetIKRotationWeight(Hint, Weight);
        

    }
}
