﻿using com.armatur.common.flags;
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

    private Action<int> onTrackChanged = (int id) => {};

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
            Debug.Log("set pt");
            TracksManager.Instance.CurrentTrack.Value.PointsTracks = value;
        }
    }

    [SerializeField]
    private Dictionary< PresetTrackView, TrackInstance> trackViews = new Dictionary<PresetTrackView, TrackInstance>();

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
        PoolManager.WarmPool(StepPrefab, 50);
        PoolManager.WarmPool(PresetTrackPrefab, 10);
        PoolManager.WarmPool(PresetTrackPrefab.GetComponent<PresetTrackView>().StepPrefab, 64*10);


        onTrackChanged = (int id) => { TrackLineChanged(id); };

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

        if (v>=1f -( 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value)))
        {
            Debug.Log("SWAP");
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
        if (TracksManager.Instance.Playing.Value && TracksManager.Instance.CurrentTrack.Value!=null)
        {
            float v = Mathf.Lerp(Slider.value, timing.Value, Time.deltaTime * bpm * Scale);


            Slider.value = v;

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
            track.OnTrackChanged += onTrackChanged;
            pointsTracks.Add(track);
           
            track.OnTrackChanged += onTrackChanged;
            GameObject newTrackView = PoolManager.Instance.spawnObject(PresetTrackPrefab);
            newTrackView.transform.SetParent(PresetsTracksHub);

            if (track.presetId == -1)
            {
                newTrackView.transform.SetAsLastSibling();
            }
            else
            {
                newTrackView.transform.SetAsFirstSibling();
            }

            newTrackView.transform.localPosition = Vector3.zero;
            newTrackView.transform.localScale = Vector3.one;
            PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
            trackView1.Init(track);
            trackView1.name = "Track_"+track.presetId;
            trackViews.Add(trackView1, currentTrack);
            
        }


        track.AddStep((4f*timing.Value * currentTrack.Steps+offset) / 64f, position);
    }

    private void TrackRemoved(PointTrack t)
    {
        if (!pointsTracks.Contains(t))
        {
            return;
        }

        PresetTrackView view = trackViews.FirstOrDefault(v => v.Key.Track == t).Key;

        trackViews.Remove(view);
        Debug.Log("destroy " + view);
        PoolManager.ReleaseObject(view.gameObject);


        t.OnTrackRemoved += TrackRemoved;
        t.OnTrackChanged += onTrackChanged;

        pointsTracks.Remove(t);
        UpdateTracks();
    }

    private void TrackLineChanged(int id)
    {
        PresetTrackView presetTrackView = trackViews.FirstOrDefault(p=>p.Key.Track.presetId == id).Key;
        presetTrackView.UpdateTrack();
    }

    private void Beat()
    {
        if (TracksManager.Instance.Playing.Value)
        {

            if (currentTrack != null)
            {

                if (timing.Value >= 1f)
                {
                    timing.SetState(0);
                    Slider.value = 0;
                }
  
                timing.SetState(timing.Value + 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value));
    
               



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
            timing.SetState(timing.Value+Time.deltaTime*bpm*Scale);
            yield return null;
        }
        Debug.Log("update track 2");
        timing.SetState(0);
        TrackChanged(TracksManager.CurrentTrack.Value);
    }


    private void SizeChanged(int v)
    {
       

        TrackChanged(TracksManager.CurrentTrack.Value);
        UpdateView();

        foreach (PresetTrackView tv in trackViews.Keys)
        {
            tv.UpdateTrack();
        }
    }

    private void UpdateView()
    {
        foreach (Transform t in StepsHub)
        {
            PoolManager.ReleaseObject(t.gameObject);
        }

        if (currentTrack != null)
        {
            for (int i = 0; i < Mathf.Pow(2, currentTrack.Size.Value + 1); i++)
            {
                GameObject newStep = PoolManager.Instance.spawnObject(StepPrefab);
                
                newStep.transform.SetParent(StepsHub);
                newStep.transform.localPosition = Vector3.zero;
                newStep.transform.localScale = Vector3.one;
            }
        }
    }

    public void UpdateTracks()
    {

        foreach (KeyValuePair<PresetTrackView, TrackInstance> pair in trackViews)
        {
            pair.Key.gameObject.SetActive(currentTrack == pair.Value);
        }

 
        if (currentTrack != null)
        {
            for (int i = currentTrack.PointsTracks.Count-1; i >=0; i--)
            {
                PointTrack track = currentTrack.PointsTracks[i];

                if (trackViews.Keys.FirstOrDefault(v=>v.Track == track) == null)
                {
                    GameObject newTrackView = PoolManager.Instance.spawnObject(PresetTrackPrefab);
                    newTrackView.transform.SetParent(PresetsTracksHub);
                    newTrackView.transform.localPosition = Vector3.zero;
                    newTrackView.transform.localScale = Vector3.one;
                    PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
                    trackView1.Init(track);
                    trackViews.Add(trackView1, currentTrack);
                }
            }
        }
    }
}

