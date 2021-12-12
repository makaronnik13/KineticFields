using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PresetStepView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {

        Vector2 local = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, GetComponentInParent<Canvas>().worldCamera, out local);
        GetComponentInParent<PresetTrackView>().RemoveStep(GetComponent<RectTransform>().anchoredPosition.x+ local.x);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            OnPointerDown(eventData);
        }
    }
}
