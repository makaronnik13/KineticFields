using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class PresetPoint : MonoBehaviour, IDragHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private AnimationCurve DynamicWidth;

    [SerializeField]
    private PresetSquare Preview;
    [SerializeField]
    private UILineRenderer Line;

    [SerializeField]
    private GameObject SelectionCircle;

    [SerializeField]
    private List<Sprite> LightningSprites = new List<Sprite>();

    private Coroutine doubleClickCoroutine = null;

    public GenericFlag<float> Volume = new GenericFlag<float>("Volume", 0);

    int t = 0;

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
        this.preset.OnPositionChanged += PositionChanged;
        Preview.Init(preset);
        BpmManager.Instance.OnQuart += Beat;
    }

    private void OnDestroy()
    {
        this.preset.OnPositionChanged -= PositionChanged;
        BpmManager.Instance.OnQuart -= Beat;
    }

    private void Beat()
    {
        t++;

        if (t >= LightningSprites.Count)
        {
            t = 0;
        }
        Line.sprite = LightningSprites[(int)t];
    }

    private void PositionChanged(Vector2 p)
    {
        transform.localPosition = p;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        Preset.Position = new Vector2(transform.localPosition.x, transform.localPosition.y);

        if (!TracksManager.Instance.Playing.Value && TracksManager.Instance.CurrentTrack.Value!=null)
        {
            float step = Mathf.RoundToInt(TrackView.Instance.Slider.value * TracksManager.Instance.CurrentTrack.Value.Steps*4f) / (TracksManager.Instance.CurrentTrack.Value.Steps*4f);

            TrackView.Instance.Slider.value = step;
            TrackView.Instance.WritePoint(Preset, transform.localPosition, 1);
        }
        //transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -150f, 150f), Mathf.Clamp(transform.localPosition.y, -150f, 150f), 0);
    }

    void Update()
    {
   

        float dist = Vector3.Distance(transform.position, PresetsLerper.Instance.RadiusView.position);

        /*Volume.SetState(Mathf.Lerp(1,0, Vector3.Distance(PresetsLerper.Instance.RadiusView.position, transform.position)/PresetsLerper.Instance.Radius.Value));
        */

        Vector2 p1 = transform.InverseTransformPoint(Center.Instance.transform.position);
        Vector2 p2 = Vector2.zero;

        Line.Points = new Vector2[2]
        {
            p1,p2
        };

        Line.color = Preset.Color2;
        //Line.Resolution = Vector2.Distance(p1,p2)/50f;
        Line.LineThickness = DynamicWidth.Evaluate(Volume.Value);
        if (Line.LineThickness<0.5f)
        {
            Line.LineThickness = 0;
        }
    }

    public void SetSelected(bool v)
    {
        SelectionCircle.SetActive(v);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (doubleClickCoroutine == null)
        {
            doubleClickCoroutine = StartCoroutine(DoubleClick());
            PresetsLerper.Instance.SelectedPreset.SetState(Preset);
        }
        else
        {
            DuplicatePreset();
        }
    }

    private IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(0.3f);
        doubleClickCoroutine = null;
    }

    private void DuplicatePreset()
    {
        PresetsLerper.Instance.DuplicateSelected();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        PresetsLerper.Instance.SelectedPreset.SetState(Preset);

        TrackView.Instance.DraggingPresets.Add(Preset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Preset.Position = new Vector2(transform.localPosition.x, transform.localPosition.y);
        TrackView.Instance.DraggingPresets.Remove(Preset);
    }
}
