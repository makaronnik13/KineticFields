using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SorceView : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(ActivePointChanged);
    }

    void OnDestroy()
    {
        KineticFieldController.Instance.ActivePoint.RemoveListener(ActivePointChanged);
    }

    private void ActivePointChanged(KineticPoint p)
    {
        this.enabled = p != null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (KineticFieldController.Instance.ActivePoint.Value==null)
        {
            return;
        }

        ISource source = null;

        if (GetComponentInParent<OscilatorView>()!=null)
        {
            source = GetComponentInParent<OscilatorView>().Oscilator;
        }
       
        if (GetComponentInParent<FrequencyGapSlider>() != null)
        {
            source = GetComponentInParent<FrequencyGapSlider>().Gap;
        }

        KineticFieldController.Instance.DraggingSource.SetState(source);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
}
