using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UniRx;
using Windows.Kinect;

public class MovementController : MonoBehaviour
{

    [SerializeField]
    private BodyVisualizer visualizer;

    private BodyView bodyView;

    [SerializeField]
    private Transform QuadTransform;

    [SerializeField]
    private AnimationCurve ScaleCurve;
    [SerializeField]
    private AnimationCurve BloomCurve;
    [SerializeField]
    private GameObject mask;

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
        visualizer.OnFirstEntered.Subscribe(bv =>
        {
            bodyView = bv;
        }).AddTo(this);

        visualizer.OnLastExit.Subscribe(_=>
        {
            bodyView = null;
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (bodyView == null)
        {
            bodyView = visualizer.GetBody(); 
        }

        mask.SetActive(bodyView != null);

        if (bodyView!=null)
        {
            Transform rightArm = bodyView.GetJoint(JointType.HandRight);
            Transform leftArm = bodyView.GetJoint(JointType.HandLeft);
            Transform head = bodyView.GetJoint(JointType.Head);

            mask.transform.position = head.position;

            Vector3 averageArmPos = (rightArm.position + leftArm.position) / 2f;

            float armDist = Vector3.Distance(rightArm.position, leftArm.position);

            Vector3 newQuadPosition = (averageArmPos - head.position) * offsetMultiplyer + startOffset;


            //Debug.Log(Vector3.SignedAngle(Vector3.left, RightArm.position - LeftArm.position, Vector3.forward)/2f);

            Quaternion quadRot = Quaternion.Euler(0, 0, -Vector3.SignedAngle(Vector3.left, rightArm.position - leftArm.position, Vector3.forward) / 5f);

            QuadTransform.localScale = Vector3.Lerp(QuadTransform.localScale, ScaleCurve.Evaluate(armDist) * Vector3.one, Time.deltaTime * scaleSpeed);
            QuadTransform.rotation = Quaternion.Lerp(QuadTransform.rotation, quadRot, Time.deltaTime * rotationSpeed);

            QuadTransform.position = Vector3.Lerp(QuadTransform.position, newQuadPosition, Time.deltaTime * offsetSpeed);

            bloomVolume.weight = BloomCurve.Evaluate(head.transform.position.z);
        }
        else
        {
            QuadTransform.localScale = Vector3.Lerp(QuadTransform.localScale, ScaleCurve.Evaluate(0) * Vector3.one, Time.deltaTime * scaleSpeed);
            QuadTransform.rotation = Quaternion.Lerp(QuadTransform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
            QuadTransform.position = Vector3.Lerp(QuadTransform.position, startOffset, Time.deltaTime * offsetSpeed);

            bloomVolume.weight = BloomCurve.Evaluate(0);
        }
    }
}
