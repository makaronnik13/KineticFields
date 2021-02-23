using com.armatur.common.flags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PresetPoint : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private PresetSquare Preview;
    [SerializeField]
    private LineRenderer Line;

    public GenericFlag<float> Volume = new GenericFlag<float>("Volume", 0);

    private KineticPreset preset;
    public KineticPreset Preset
    {
        get
        {
            return preset;
        }
    }

    public void Init(KineticPreset preset)
    {
        this.preset = preset;
        Preview.Init(preset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -150f, 150f), Mathf.Clamp(transform.localPosition.y, -150f, 150f), 0);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, PresetsLerper.Instance.RadiusView.position);

        /*Volume.SetState(Mathf.Lerp(1,0, Vector3.Distance(PresetsLerper.Instance.RadiusView.position, transform.position)/PresetsLerper.Instance.Radius.Value));

        Line.SetPosition(0, transform.parent.position);
        Line.SetPosition(1, transform.position);
        Line.widthCurve = new AnimationCurve(new Keyframe[]{
            new Keyframe(0,Volume.Value*100f),
            new Keyframe(1,Volume.Value*100f)
        });*/
    }
}
