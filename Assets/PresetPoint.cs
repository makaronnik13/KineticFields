using com.armatur.common.flags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PresetPoint : MonoBehaviour,  IDragHandler
{
    [SerializeField]
    private PresetSquare Preview;

    public GenericFlag<float> Volume = new GenericFlag<float>("Volume", 0);

    public void Init(KineticPreset preset)
    {
        Preview.Init(preset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -150f, 150f), Mathf.Clamp(transform.localPosition.y, -150f, 150f), 0);
    }

    void Update()
    {
        float dist = Mathf.Clamp(transform.localPosition.magnitude,0,120f);
        Volume.SetState(Mathf.Lerp(1,0, dist/120f));
    }
}
