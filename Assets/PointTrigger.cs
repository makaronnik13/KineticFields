using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointTrigger : MonoBehaviour
{
    [SerializeField]
    private KineticPoint3dAnchor Point3d;

    private Coroutine doubleClickCoroutine;
    private bool dragging = false;



    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
            if (doubleClickCoroutine != null)
        {
            Point3d.Toggle();
            StopCoroutine(doubleClickCoroutine);
            doubleClickCoroutine = null;
        }
        else
        {
            doubleClickCoroutine = StartCoroutine(DoubleClick());
        }

        Point3d.Select();
        dragging = true;
    }

    private IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(0.3f);
        doubleClickCoroutine = null;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Point3d.Drag();


    }
}
