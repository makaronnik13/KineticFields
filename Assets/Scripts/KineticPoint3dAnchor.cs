using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticPoint3dAnchor : MonoBehaviour
{
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

            transform.position = Vector3.Lerp(transform.position, Point2d.Point.Position, Time.deltaTime*2f);

          
            transform.localScale = Vector3.one * Point2d.Point.Radius.Value.Value;
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
