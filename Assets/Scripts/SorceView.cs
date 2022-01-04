using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SorceView : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Source source;

    void OnEnable()
    {
        if (GetComponentInParent<OscilatorView>() != null)
        {
            source = GetComponentInParent<OscilatorView>().Oscilator;
        }

        if (GetComponentInParent<FrequencyGapSlider>() != null)
        {
            source = GetComponentInParent<FrequencyGapSlider>().Gap;
        }
    }

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

        Debug.Log(source);

        KineticFieldController.Instance.DraggingSource.SetState(source);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KineticFieldController.Instance.SelectedSource.SetState(source);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KineticFieldController.Instance.SelectedSource.SetState(null);
    }
}
