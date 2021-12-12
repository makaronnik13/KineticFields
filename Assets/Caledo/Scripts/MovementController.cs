using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Transform Head, LeftArm, RightArm;

    [SerializeField]
    private Transform QuadTransform;

    [SerializeField]
    private AnimationCurve ScaleCurve;
    [SerializeField]
    private AnimationCurve BloomCurve;


    [SerializeField]
    private float scaleSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private float offsetMultiplyer = 1f;

    [SerializeField]
    private float offsetSpeed = 2f;

    private Vector3 startOffset;

    [SerializeField]
    private Volume bloomVolume;
    [SerializeField]
    private Volume colorVolume;

    private void Start()
    {
        startOffset = QuadTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 averageArmPos = (RightArm.position + LeftArm.position) / 2f;

        float armDist = Vector3.Distance(RightArm.position, LeftArm.position);

        Vector3 newQuadPosition = (averageArmPos- Head.position) *offsetMultiplyer+startOffset;


        //Debug.Log(Vector3.SignedAngle(Vector3.left, RightArm.position - LeftArm.position, Vector3.forward)/2f);

        Quaternion quadRot = Quaternion.Euler(0, 0, -Vector3.SignedAngle(Vector3.left, RightArm.position-LeftArm.position, Vector3.forward)/5f) ;

        QuadTransform.localScale = Vector3.Lerp(QuadTransform.localScale, ScaleCurve.Evaluate(armDist) * Vector3.one, Time.deltaTime*scaleSpeed);
        QuadTransform.rotation = Quaternion.Lerp(QuadTransform.rotation, quadRot, Time.deltaTime*rotationSpeed);

        QuadTransform.position = Vector3.Lerp(QuadTransform.position, newQuadPosition, Time.deltaTime*offsetSpeed);

        bloomVolume.weight = BloomCurve.Evaluate(Head.transform.position.z);
    }
}
