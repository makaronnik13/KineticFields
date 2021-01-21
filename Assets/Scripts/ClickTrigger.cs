using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickTrigger : MonoBehaviour
{
    public UnityEvent OnClick;

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            OnClick.Invoke();
        }
    }
}
