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
    private float LerpCoef = 1f;

    [SerializeField]
    public Slider Slider;

    [SerializeField]
    private Transform StepsHub, PresetsTracksHub;

    [SerializeField]
    private GameObject StepPrefab, PresetTrackPrefab;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private TracksManager TracksManager;

    public float Scale = 0.001f;

    [SerializeField]
    public List<PointTrack> pointsTracks
    {
        get
        {
            if (TracksManager.Instance.CurrentTrack.Value == null)
            {
                return new List<PointTrack>();
            }
            return TracksManager.Instance.CurrentTrack.Value.PointsTracks;
        }
        set
        {
            TracksManager.Instance.CurrentTrack.Value.PointsTracks = value;
        }
    }

    [SerializeField]
    private List<PresetTrackView> trackViews = new List<PresetTrackView>();

    private  TrackInstance currentTrack;
    private GenericFlag<float> timing = new GenericFlag<float>("timing", 0);
    private int bpm;

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
        FindObjectOfType<BpmManager>().Bpm.AddListener(BpmChanged);
        TracksManager.Instance.Playing.AddListener(PlayingStateChanged);
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
        ApplyPositions();
        if (v>=1)
        {
            TracksManager.Instance.RandomSwap();
        }
    }

    private void ApplyPositions()
    {
        foreach (PointTrack tr in pointsTracks)
        {
            KineticPreset preset = KineticFieldController.Instance.Session.Value.MainPreset;
            if (tr.presetId>=0)
            {
                preset = KineticFieldController.Instance.Session.Value.Presets[tr.presetId];
            }
            if (DraggingPresets.FirstOrDefault(p => preset  == p) != null)
            {

            }
            else
            {



                float vv =  (Slider.value * currentTrack.Steps*4) / 64f;


                Vector2 newPos = tr.GetPosition(vv);
                //newPos = Vector2.Lerp(KineticFieldController.Instance.Session.Value.Presets[tr.presetId].Position, newPos, 0.3f);

                preset.Position = newPos;

            }
        }
    }

    private void SliderValueChangedEdtitMode(float v)
    {
        
        timing.SetState(v);
        ApplyPositions();
    }

    private void Update()
    {
        if (TracksManager.Instance.Playing.Value)
        {
            Slider.value = Mathf.Lerp(Slider.value, timing.Value, Time.deltaTime * bpm * Scale);
            if (timing.Value == 0)
            {
                Slider.value = 0;
            }

        }

        PresetTrackView mainTrack = trackViews.FirstOrDefault(t => t.Track.presetId == -1);
        if (mainTrack)
        {
            mainTrack.transform.SetAsLastSibling();
        }
    }

    public void WritePoint(KineticPreset preset, Vector2 position, int offset = 0)
    {
        int id = KineticFieldController.Instance.Session.Value.Presets.IndexOf(preset);
        PointTrack track = pointsTracks.FirstOrDefault(t=>t.presetId == id);


        if (track == null)
        {
            track = new PointTrack(id);
            track.OnTrackRemoved += TrackRemoved;
            pointsTracks.Add(track);
            track.OnTrackChanged += ()=> { TrackLineChanged(id); };
            GameObject newTrackView = Instantiate(PresetTrackPrefab);
            newTrackView.transform.SetParent(PresetsTracksHub);
            newTrackView.transform.localPosition = Vector3.zero;
            newTrackView.transform.localScale = Vector3.one;
            PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
            trackView1.Init(track);
            trackViews.Add(trackView1);
            
        }


        track.AddStep((4f*timing.Value * currentTrack.Steps+offset) / 64f, position);
    }

    private void TrackRemoved(PointTrack t)
    {
        pointsTracks.Remove(t);
        PresetTrackView view = trackViews.FirstOrDefault(v=>v.Track == t);
        trackViews.Remove(view);
        Destroy(view);
    }

    private void TrackLineChanged(int id)
    {
        PresetTrackView presetTrackView = trackViews.FirstOrDefault(p=>p.Track.presetId == id);
        presetTrackView.UpdateTrack();
    }

    private void Beat()
    {
        if (TracksManager.Instance.Playing.Value)
        {

            if (currentTrack != null)
            {
                timing.SetState(timing.Value + 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value));

                if (timing.Value > 1f)
                {
                    timing.SetState(0);
                }



                foreach (KineticPreset preset in DraggingPresets)
                {
                    WritePoint(preset, PresetsLerper.Instance.GetPosition(preset));
                }

            }
        }
        else
        {

        }
    }

    private void BpmChanged(int bpm)
    {
        this.bpm = bpm;
        TrackChanged(TracksManager.CurrentTrack.Value);
    }

    private void TrackChanged(TrackInstance track)
    {
        timing.SetState(0);

        if (track != currentTrack)
        {
            if (currentTrack!=null)
            {
                currentTrack.Size.RemoveListener(SizeChanged);
            }
            currentTrack = track;
            if (currentTrack != null)
            {
                currentTrack.Size.AddListener(SizeChanged);
            }
            UpdateView();
            UpdateTracks();
        }

        View.SetActive(track!=null);
    }

    private IEnumerator UpdateTrack()
    {
        timing.SetState(0);
        while (timing.Value < 1f)
        {
            Debug.Log(timing.Value);
            timing.SetState(timing.Value+Time.deltaTime*bpm*Scale);
            yield return null;
        }
        timing.SetState(0);
        TrackChanged(TracksManager.CurrentTrack.Value);
    }


    private void SizeChanged(int v)
    {
        TrackChanged(TracksManager.CurrentTrack.Value);
        UpdateView();

        foreach (PresetTrackView tv in trackViews)
        {
            tv.UpdateTrack();
        }
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
                GameObject newStep = Instantiate(StepPrefab);
                newStep.transform.SetParent(StepsHub);
                newStep.transform.localPosition = Vector3.zero;
                newStep.transform.localScale = Vector3.one;
            }
        }
    }

    public void UpdateTracks()
    {
        foreach (PresetTrackView tw in trackViews)
        {
            tw.Track.OnTrackRemoved -= TrackRemoved;
            //tw.Track.OnTrackChanged -= TrackChanged;
            Destroy(tw.gameObject);
        }

        trackViews.Clear();
        pointsTracks.Clear();

        if (currentTrack != null)
        {
            Debug.Log(currentTrack.PointsTracks.Count+"/"+currentTrack.Color);

            foreach (PointTrack track in currentTrack.PointsTracks)
            {
                track.OnTrackRemoved += TrackRemoved;
                pointsTracks.Add(track);
                track.OnTrackChanged += () => { TrackLineChanged(track.presetId); };


                GameObject newTrackView = Instantiate(PresetTrackPrefab);
                newTrackView.transform.SetParent(PresetsTracksHub);
                newTrackView.transform.localPosition = Vector3.zero;
                newTrackView.transform.localScale = Vector3.one;
                PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
                trackView1.Init(track);
                trackViews.Add(trackView1);
            }
        }
    }
}

