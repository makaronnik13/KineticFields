using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Center : Singleton<Center>, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private float Sensivity = 1f;

    private bool writing;

    private int beatCount = 0;

    private KineticPreset Preset
    {
        get
        {
            if (KineticFieldController.Instance.Session.Value == null)
            {
                return null;
            }
            return KineticFieldController.Instance.Session.Value.MainPreset;
        }
    }

    void Start()
    {
        if (Preset!=null)
        {
            Preset.OnPositionChanged += PositionChanged;
        }
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
        if (PresetsLerper.Instance.Lerping.Value)
        {
            float rad = PresetsLerper.Instance.Radius.Value + Input.mouseScrollDelta.y * Sensivity;
            rad = Mathf.Clamp(rad, 50, 250);
            PresetsLerper.Instance.Radius.SetState(rad);
        }
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        TrackView.Instance.DraggingPresets.Add(Preset);
        if (TracksManager.Instance.Playing.Value)
        {
            BpmManager.Instance.OnBeat += Beat;
        }
    }

    private void Beat()
    {
        TrackView.Instance.ResetTime();
        writing = true;
        BpmManager.Instance.OnBeat -= Beat;
        BpmManager.Instance.OnBeat += Beat2;
    }

    private void Beat2()
    {
        beatCount++;
        if (TracksManager.Instance.CurrentTrack.Value!=null && beatCount >= TracksManager.Instance.CurrentTrack.Value.Steps)
        {
            TrackView.Instance.DraggingPresets.Remove(Preset);
           BpmManager.Instance.OnBeat -= Beat2;
            writing = false;
            beatCount = 0;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        writing = false;
        BpmManager.Instance.OnBeat -= Beat2;
        BpmManager.Instance.OnBeat -= Beat;
        if (Preset!=null)
        {
            Preset.Position = new Vector2(transform.localPosition.x, transform.localPosition.y);
        }

        TrackView.Instance.DraggingPresets.Remove(Preset);
        SessionsManipulator.Instance.Autosave();
    }
}
