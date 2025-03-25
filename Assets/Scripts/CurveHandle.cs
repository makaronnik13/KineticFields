using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurveHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Keyframe kf;
    public int Index;

    private Action<CurveHandle> OnChanged = (v) => { };

    void Update()
    {
        if (CurveRedactor.Instance.EditingPoint.Value == this)
        {
            transform.localScale = Vector3.one * 1.5f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CurveRedactor.Instance.EditingPoint.SetState(this);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData.position);
    }


    public void SetPosition(Vector2 mousePosition)
    {
        RectTransform rt = GetComponent<RectTransform>();
        this.transform.position = mousePosition;
        rt.anchoredPosition = new Vector2(Mathf.Clamp(rt.anchoredPosition.x, 0, 300), Mathf.Clamp(rt.anchoredPosition.y, -100, 100));

        if (CurveRedactor.Instance.handles.IndexOf(this) == 0)
        {
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
        }

        if (CurveRedactor.Instance.handles.IndexOf(this) == CurveRedactor.Instance.handles.Count - 1)
        {
            rt.anchoredPosition = new Vector2(300, rt.anchoredPosition.y);
        }

        if (CurveRedactor.Instance.handles.IndexOf(this) != 0 && CurveRedactor.Instance.handles.IndexOf(this) != CurveRedactor.Instance.handles.Count - 1)
        {
            rt.anchoredPosition = new Vector2(Mathf.Clamp(rt.anchoredPosition.x, 1, 299), Mathf.Clamp(rt.anchoredPosition.y, -100, 100));
        }


        kf.time = rt.anchoredPosition.x / 300f;
        kf.value = rt.anchoredPosition.y / 100f;
        OnChanged.Invoke(this);
    }


    public void SetValue(float v)
    {
        kf.value = Mathf.Clamp(v, -1f, 1f);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(kf.time * 300, kf.value * 100f);
        OnChanged.Invoke(this);
    }

    public void SetTime(float v)
    {
        kf.time = Mathf.Clamp(v, 0f, 1f);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(kf.time*300, kf.value*100f);
        OnChanged.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CurveRedactor.Instance.EditingPoint.SetState(this);
    }

    public void Init(int index, Keyframe kf, Action<CurveHandle> OnChanged)
    {
        this.Index = index;
        this.OnChanged = OnChanged;
        this.kf = kf;
    }

   
}
