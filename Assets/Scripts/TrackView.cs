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
    private GameObject StepPrefab, PresetTrackPrefab;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private TracksManager TracksManager;

    public float Scale = 0.001f;

    private Action<int> onTrackChanged = (int id) => {};

    [SerializeField]
    public PointTrack pointsTrack
    {
        get
        {
            if (TracksManager.Instance.CurrentTrack.Value == null)
            {
                return null;
            }
            return TracksManager.Instance.CurrentTrack.Value.PointsTrack;
        }
        set
        {
            TracksManager.Instance.CurrentTrack.Value.PointsTrack = value;
        }
    }

    [SerializeField]
    private Dictionary<PresetTrackView, TrackInstance> trackViews = new Dictionary<PresetTrackView, TrackInstance>();

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


        onTrackChanged = (int id) => { TrackLineChanged(id); };

        TracksManager.CurrentTrack.AddListener(TrackChanged);
        FindObjectOfType<BpmManager>().OnQuart += Beat;
        FindObjectOfType<BpmManager>().Bpm.AddListener(BpmChanged);
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
        ApplyPositions();

        if (v>=1f-0.001f)// -( 1f / Mathf.Pow(2, 3 + currentTrack.Size.Value)))
        {
            TracksManager.Instance.RandomSwap();
            ResetTime();
        }
    }

    private void ApplyPositions()
    {


            KineticPreset preset = KineticFieldController.Instance.Session.Value.MainPreset;
            

            if (DraggingPresets.FirstOrDefault(p => preset  == p) != null)
            {

            }
            else
            {
                float vv =  (Slider.value * currentTrack.Steps*4) / 64f;
                Vector2 newPos = preset.Position;
                if (pointsTrack.GetPosition(vv, out newPos))
                {
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


        if (!trackViews.ContainsValue(currentTrack))
        {
            pointsTrack = new PointTrack(id);
            pointsTrack.OnTrackRemoved += TrackRemoved;
            pointsTrack.OnTrackChanged += onTrackChanged;
            pointsTrack.OnTrackChanged += onTrackChanged;
            GameObject newTrackView = PresetTrackPrefab.Spawn();
            newTrackView.transform.SetParent(PresetsTracksHub);
            newTrackView.transform.localPosition = Vector3.zero;
            newTrackView.transform.localScale = Vector3.one;
            PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
            trackView1.Init(pointsTrack);
            trackView1.name = "Track";
            trackViews.Add(trackView1, currentTrack);
            
        }
        else
        {
           trackViews.FirstOrDefault(t=>t.Key.Track == pointsTrack).Key.Init(pointsTrack);
        }

        pointsTrack.AddStep((4f*timing.Value * currentTrack.Steps+offset) / 64f, position);
    }

    private void TrackRemoved(PointTrack t)
    {


        PresetTrackView view = trackViews.FirstOrDefault(v => v.Key.Track == t).Key;

        trackViews.Remove(view);
        Debug.Log("destroy " + view);
        view.gameObject.Recycle();


        t.OnTrackRemoved += TrackRemoved;
        t.OnTrackChanged += onTrackChanged;

        pointsTrack = null;
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
                   // ResetTime();
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

    public void ResetTime()
    {
        timing.SetState(0);
        Slider.value = 0;
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
                currentTrack.CurrentRepeat.SetState(0);
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
        Length.text = Mathf.Pow(2, 1 + v).ToString();

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
          
                PointTrack track = pointsTrack;

                if (track!=null && trackViews.Keys.FirstOrDefault(v=>v.Track == track) == null && track.PositionSteps.FirstOrDefault(s=>s.HasKey.Value)!=null)
                {
                    GameObject newTrackView = PresetTrackPrefab.Spawn();
                    newTrackView.transform.SetParent(PresetsTracksHub);
                    newTrackView.transform.localPosition = Vector3.zero;
                    newTrackView.transform.localScale = Vector3.one;
                    PresetTrackView trackView1 = newTrackView.GetComponent<PresetTrackView>();
                    trackView1.Init(track);
                    trackViews.Add(trackView1, currentTrack);
                }

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

}

