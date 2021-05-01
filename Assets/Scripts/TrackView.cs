using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrackView : Singleton<TrackView>
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Length;

    [SerializeField]
    private TMPro.TMP_InputField RepeatCount;

    [SerializeField]
    private float LerpCoef = 1f;

    [SerializeField]
    public Slider Slider;

    [SerializeField]
    private Transform StepsHub, PresetsTracksHub;

    [SerializeField]
    private GameObject StepPrefab;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private TracksManager TracksManager;

    public float Scale = 0.001f;


    [SerializeField]
    private PresetTrackView RadiusTrackView, PositionTrackView;

    private  TrackInstance currentTrack;
    private GenericFlag<float> timing = new GenericFlag<float>("timing", 0);
    private int bpm
    {
        get
        {
            return BpmManager.Instance.Bpm.Value;
        }
    }

    public List<KineticPreset> DraggingPresets = new List<KineticPreset>();

    public bool Sliding = false;

    public void SetSliding(bool v)
    {
        Sliding = v;
    }

    private void Start()
    {
        TracksManager.CurrentTrack.AddListener(TrackChanged);
        FindObjectOfType<BpmManager>().OnQuart += Beat;
        TracksManager.Instance.Playing.AddListener(PlayingStateChanged);

        RepeatCount.onValueChanged.AddListener(RepeatCountChanged);
    }

    private void RepeatCountChanged(string s)
    {
        currentTrack.RepeatCount.SetState(int.Parse(s));
    }

    private void PlayingStateChanged(bool v)
    {
        Slider.onValueChanged.RemoveAllListeners();

        Slider.interactable = !v;

        if (Slider.interactable)
        {
            Slider.onValueChanged.AddListener(SliderValueChangedEdtitMode);
        }
        else
        {
            Slider.onValueChanged.AddListener(SliderValueChangedPlaymode);
        }
    }

    private void SliderValueChangedPlaymode(float v)
    {
        ApplyPositionAndRadius();

        if (v>=1f-0.001f)// -( 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value)))
        {
            TracksManager.Instance.RandomSwap();
            ResetTime();
        }
    }

    private void ApplyPositionAndRadius()
    {
        if (KineticFieldController.Instance.Session.Value==null)
        {
            return;
        }

            KineticPreset preset = KineticFieldController.Instance.Session.Value.MainPreset;


            if (DraggingPresets.FirstOrDefault(p => preset  == p) != null)
            {

            }
            else
            {
                float vv =  (Slider.value * currentTrack.Steps*4) / 64f;
                Vector2 newPos = preset.Position;
                if (TracksManager.Instance.CurrentTrack.Value.PositionTrack.GetPosition(vv, out newPos))
                {
                preset.Position = newPos;
                }

                float newRadius = PresetsLerper.Instance.Radius.Value;

            if (TracksManager.Instance.CurrentTrack.Value.RadiusTrack.GetRadius(vv, out newRadius))
            {
                PresetsLerper.Instance.Radius.SetState(newRadius);
            }
            }
    }

    private void SliderValueChangedEdtitMode(float v)
    {


        timing.SetState(v);

        ApplyPositionAndRadius();
    }

    private void Update()
    {
        if (TracksManager.Instance.Playing.Value && TracksManager.Instance.CurrentTrack.Value!=null)
        {
            float v = Mathf.Lerp(Slider.value, timing.Value, Time.deltaTime * bpm * Scale);

            Slider.value = v;

        }

    
    }

    public void WriteRadius(float radius, int offset = 0)
    {
        if (currentTrack==null)
        {
            return;
        }
        currentTrack.RadiusTrack.AddStep((4f * Slider.value * currentTrack.Steps + offset) / 64f, radius);
    }

    public void WritePosition(Vector2 position, int offset = 0)
    {
        if (currentTrack==null)
        {
            return;
        }
        currentTrack.PositionTrack.AddStep((4f*Slider.value * currentTrack.Steps+offset) / 64f, position);
    }


    private void Beat()
    {
        if (TracksManager.Instance.Playing.Value)
        {

            if (currentTrack != null)
            {

                timing.SetState(timing.Value + 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value));
            }
        }
    }

    public void ResetTime()
    {
        timing.SetState(0);
        Slider.value = 0;
    }

    public void ResetTrack()
    {
        if (TracksManager.Instance.Playing.Value)
        {
            TrackChanged(TracksManager.CurrentTrack.Value);
        }
    }

    private void TrackChanged(TrackInstance track)
    {
   
        if (TracksManager.Instance.Playing.Value)
        {
            timing.SetState(0);
        }
        else
        {
            timing.SetState(Slider.value);
        }


        if (track != currentTrack)
        {
            if (currentTrack!=null)
            {
                currentTrack.CurrentRepeat.SetState(0);
                currentTrack.Size.RemoveListener(SizeChanged);
                currentTrack.PositionTrack.OnTrackChanged -= UpdateView;
                currentTrack.RadiusTrack.OnTrackChanged -= UpdateView;
            }

            currentTrack = track;

            if (currentTrack != null)
            {
                RadiusTrackView.Init(currentTrack.RadiusTrack);
                PositionTrackView.Init(currentTrack.PositionTrack);
                currentTrack.PositionTrack.OnTrackChanged += UpdateView;
                currentTrack.RadiusTrack.OnTrackChanged += UpdateView;
                currentTrack.Size.AddListener(SizeChanged);        
            }
            UpdateView();
        }

        View.SetActive(track!=null);
    }

    private IEnumerator UpdateTrack()
    {

        timing.SetState(0);
        while (timing.Value < 1f)
        {
            timing.SetState(timing.Value+Time.deltaTime*bpm*Scale);
            yield return null;
        }
        timing.SetState(0);
        TrackChanged(TracksManager.CurrentTrack.Value);
    }


    private void SizeChanged(int v)
    {
        Length.text = Mathf.Pow(2, 1 + v).ToString();

        TrackChanged(TracksManager.CurrentTrack.Value);
        UpdateView();
    }

    private void UpdateView()
    {
        foreach (Transform t in StepsHub)
        {
            Destroy(t.gameObject);
        }


        if (currentTrack != null)
        {
            for (int i = 0; i < Mathf.Pow(2, currentTrack.Size.Value + 1); i++)
            {
                GameObject newStep = StepPrefab.Spawn();           
                newStep.transform.SetParent(StepsHub);
                newStep.transform.localPosition = Vector3.zero;
                newStep.transform.localScale = Vector3.one;
            }

            RepeatCount.text = currentTrack.RepeatCount.Value.ToString();

            RadiusTrackView.UpdateTrack();
            PositionTrackView.UpdateTrack();


            RadiusTrackView.gameObject.SetActive(currentTrack.RadiusTrack.RadiusSteps.Count > 0);
            PositionTrackView.gameObject.SetActive(currentTrack.PositionTrack.PositionSteps.Count > 0);
        }

    }

   
    public void SizeClicked()
    {
        currentTrack.Size.SetState(currentTrack.Size.Value + 1);
        if (currentTrack.Size.Value > 4)
        {
            currentTrack.Size.SetState(0);
        }
    }

    private void OnDestroy()
    {
        if (currentTrack!=null)
        {
            currentTrack.PositionTrack.OnTrackChanged -= UpdateView;
            currentTrack.RadiusTrack.OnTrackChanged -= UpdateView;
        }
    }

}

