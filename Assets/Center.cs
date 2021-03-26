using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Center : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private float Sensivity = 1f;

    private KineticPreset Preset
    {
        get
        {
            return KineticFieldController.Instance.Session.Value.MainPreset;
        }
    }

    void Start()
    {
        Preset.OnPositionChanged += PositionChanged;
    }

    private void PositionChanged(Vector2 p)
    {
        transform.localPosition = p;
    }


    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        if (!TracksManager.Instance.Playing.Value && TracksManager.Instance.CurrentTrack.Value != null)
        {
   

            float step = Mathf.RoundToInt(TrackView.Instance.Slider.value * TracksManager.Instance.CurrentTrack.Value.Steps*4)/ (TracksManager.Instance.CurrentTrack.Value.Steps*4f);


            TrackView.Instance.Slider.value = step;
            TrackView.Instance.WritePoint(Preset, transform.localPosition, 1);
        }
    }

    private void Update()
    {
        float rad = PresetsLerper.Instance.Radius.Value + Input.mouseScrollDelta.y * Sensivity;
        rad = Mathf.Clamp(rad, 50, 250);
        PresetsLerper.Instance.Radius.SetState(rad);
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        TrackView.Instance.DraggingPresets.Add(Preset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Preset.Position = new Vector2(transform.localPosition.x, transform.localPosition.y);
        TrackView.Instance.DraggingPresets.Remove(Preset);
    }
}
