using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Center : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private float Sensivity = 1f;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
       
    }

    private void Update()
    {
        float rad = PresetsLerper.Instance.Radius.Value + Input.mouseScrollDelta.y * Sensivity;
        rad = Mathf.Clamp(rad, 50, 250);
        PresetsLerper.Instance.Radius.SetState(rad);
    }
}
