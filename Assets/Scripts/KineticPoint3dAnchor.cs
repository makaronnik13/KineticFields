using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticPoint3dAnchor : MonoBehaviour
{
    public float LerpSpeed
    {
        get
        {
            return DefaultResources.Settings.PointsLerpSpeed3d;
        }
    } 

    [SerializeField]
    private KineticPoint Point2d;

    [SerializeField]
    private float Deep;

    [SerializeField]
    private GameObject BlackView;

   

    private void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(ActivePoitChanged);
    }

    private void ActivePoitChanged(KineticPoint point)
    {
        transform.GetChild(0).gameObject.SetActive(point == Point2d);
    }

    void Update()
    {
        if (!PresetsLerper.Instance.Lerping.Value)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                BlackView.transform.localScale = Vector3.Lerp(BlackView.transform.localScale, Vector3.one * 3, Time.deltaTime * 3);
            }
            else
            {
                BlackView.transform.localScale = Vector3.Lerp(BlackView.transform.localScale, Vector3.zero, Time.deltaTime * 3);
            }

            if (KineticFieldController.Instance.ActivePoint.Value == Point2d)
            {
                Point2d.Point.Deep.SetValue(KineticFieldController.Instance.ActivePoint.Value.Point.Deep.BaseValue.Value - Input.mouseScrollDelta.y * 0.3f);
            }
        }

        if (Point2d && Point2d.Point!=null)
        {
            //Debug.Log(KineticFieldController.Instance.Session.Value.GeneralAnchor.Value.Value - 4f);
            Vector3 newPos = Point2d.Point.Position;
            if (Point2d.Point.Id!=0)
            {
                newPos = new Vector3(newPos.x, newPos.y, newPos.z* KineticFieldController.Instance.Session.Value.GeneralScale.Value.Value);

                newPos += Vector3.forward * (KineticFieldController.Instance.Session.Value.GeneralAnchor.Value.Value - 4f);
            }


            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime*LerpSpeed);

          
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * Point2d.Point.Radius.Value.Value,Time.deltaTime*LerpSpeed);
        }

        Point2d.transform.position = Camera.main.transform.position - (Camera.main.transform.position - transform.position).normalized;

       
       
    }

    public void Toggle()
    {
        Point2d.Point.Active.SetState(!Point2d.Point.Active.Value);
    }

    public void Select()
    {
        KineticFieldController.Instance.ActivePoint.SetState(Point2d);
    }


    public void Drag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        curPosition.z = Point2d.Point.Deep.Value.Value;

        Point2d.Point.Position = curPosition;

        BlackView.transform.localScale = Vector3.Lerp(BlackView.transform.localScale, Vector3.one*1.5f, Time.deltaTime * 3);

    }
}
