using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TracksManager : Singleton<TracksManager>
{
    [SerializeField]
    private GameObject SavePanel, LoadPanel;

    [SerializeField]
    private GameObject TrackBtnPrefab, TrackLibBtnPrefab;

    [SerializeField]
    private Animator PlayStopBtnAnimator;

    [SerializeField]
    private Image PlayStopIcon, ShuffleIcon;

    [SerializeField]
    private Sprite PlaySprte, StopSprite, ShuffleSprite, LoopSprite;

    [SerializeField]
    private TMPro.TMP_InputField LibNameInput;

    [SerializeField]
    private Transform LibsHub;

    [SerializeField]
    private GameObject PlayStopBtn;

    private TrackInstance nextTrack = null;

    [SerializeField]
    public GenericFlag<bool> Playing = new GenericFlag<bool>("Playing", false);

    [SerializeField]
    public GenericFlag<bool> Shufle = new GenericFlag<bool>("Shuffle", false);

    //[SerializeField]
    //private TMPro.TextMeshProUGUI ChangeRate;

    public GenericFlag<TrackLib> CurrentLib = new GenericFlag<TrackLib>("TrackLib", null);

    public GenericFlag<TrackInstance> CurrentTrack = new GenericFlag<TrackInstance>("CurrentTrack", null);

    private List<GameObject> trackBtns = new List<GameObject>();

    private TrackLib _lastLib;

    private void Awake()
    {
        if (CurrentLib.Value == null)
        {
            CurrentLib.SetState(new TrackLib("NewTrackLib_" + (SessionsManipulator.Instance.TrackLibs.Count + 1)));
        }
        CurrentLib.AddListener(LibChanged);
        FindObjectOfType<BpmManager>().OnBeat += Beat;
        Playing.AddListener(PlayingStateChanged);
        Shufle.AddListener(ShufleStateChanged);
        PresetsLerper.Instance.OnPresetDeleted += PresetDeleted;
    }

    private void ShufleStateChanged(bool v)
    {
        if (v)
        {
            ShuffleIcon.sprite = ShuffleSprite;
        }
        else
        {
            ShuffleIcon.sprite = LoopSprite;
        }
    }

    private void PresetDeleted(KineticPreset p)
    {
        int id = KineticFieldController.Instance.Session.Value.Presets.IndexOf(p);

        for (int i = CurrentLib.Value.Tracks.Count-1; i>=0; i--)
        {
            PointTrack pTrack = CurrentLib.Value.Tracks[i].PointsTracks.FirstOrDefault(t => t.presetId == id);
            if (pTrack != null)
            {
                pTrack.OnTrackRemoved(pTrack);
            }
        }

    }

    public void CreateNew()
    {
        LibNameInput.text = CurrentLib.Value.Name;
        SaveLib();

        CurrentLib.SetState(new TrackLib("NewTrackLib_"+(SessionsManipulator.Instance.TrackLibs.Count+1)));
    }

    private void Beat()
    {
        if (nextTrack!=null)
        {
            CurrentTrack.SetState(nextTrack);
            nextTrack = null;
        }
    }

    public void SetTrack(TrackInstance track)
    {
        if (track == null)
        {
            CurrentTrack.SetState(null);
        }
        nextTrack = track;
  
    }

    private void LibChanged(TrackLib lib)
    {
        PlayStopBtn.SetActive(CurrentLib.Value.Tracks.Count > 0);

        if (_lastLib==lib)
        {
            return;
        }

        Playing.SetState(false);

        if (lib.Tracks.Count>0)
        {
            if (CurrentTrack.Value!=null)
            {
                Debug.Log(CurrentTrack.Value.PointsTracks.Count + "/" + CurrentTrack.Value.Color+" F");
            }
           

            CurrentTrack.SetState(lib.Tracks[0]);
        }
        else
        {
            CurrentTrack.SetState(null);
        }


        foreach (GameObject t in trackBtns)
        {
            Destroy(t);
        }

        trackBtns.Clear();
        foreach (TrackInstance track in lib.Tracks)
        {
            CreateTrackBtn(track);
        }

            _lastLib = lib;

            _lastLib.ChangeRate.AddListener(LibRateChanged);
   

      
    }

    private void LibRateChanged(int v)
    {
       
    }

    private void PlayingStateChanged(bool v)
    {
        PlayStopBtnAnimator.SetBool("Show", v);

        if (v)
        {
            PlayStopIcon.sprite = PlaySprte;
            PlayStopIcon.transform.parent.GetComponent<Image>().color = new Color(0, 1f, 0, 0.02f);
        }
        else
        {
            PlayStopIcon.sprite = StopSprite;
            PlayStopIcon.transform.parent.GetComponent<Image>().color = new Color(1f, 0f, 0, 0.02f);
        }
    }

    public void OnEnable()
    {
        if (CurrentLib.Value == null)
        {
            CurrentLib.SetState(new TrackLib("NewTrackLib"));
        }
    }

    private void OnDisable()
    {
        CurrentTrack.SetState(null);
    }

    public void DeleteCurrentTrack()
    {
        CurrentLib.Value.Tracks.Remove(CurrentTrack.Value);
        Destroy(GetComponentsInChildren<SingleTrackView>().FirstOrDefault(v=>v.Track == CurrentTrack.Value).gameObject);
        LibChanged(CurrentLib.Value);
        CurrentTrack.SetState(CurrentLib.Value.Tracks.FirstOrDefault());
    }

    public void CreateTrack()
    {
        int id = UnityEngine.Random.Range(0, DefaultResources.TrackSprites.Count - 1);
        TrackInstance newTrack = new TrackInstance(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f), DefaultResources.TrackSprites[id]);

        CreateTrackBtn(newTrack);

        CurrentLib.Value.Tracks.Add(newTrack);

        LibChanged(CurrentLib.Value);

        nextTrack = newTrack;
    }

    private void CreateTrackBtn(TrackInstance newTrack)
    {
        GameObject newTrackBtn = Instantiate(TrackBtnPrefab);
        trackBtns.Add(newTrackBtn);
        newTrackBtn.transform.SetParent(transform);
        newTrackBtn.transform.localPosition = Vector3.zero;
        newTrackBtn.transform.localScale = Vector3.one;
        newTrackBtn.GetComponent<SingleTrackView>().Init(newTrack);
        newTrackBtn.transform.SetSiblingIndex(CurrentLib.Value.Tracks.Count+4);
    }

    public void RandomSwap()
    {
        if (!Shufle.Value)
        {
            return;
        }

        int lastId = CurrentLib.Value.Tracks.IndexOf(CurrentTrack.Value);

        if (lastId == -1)
        {
            lastId = UnityEngine.Random.Range(0, CurrentLib.Value.Tracks.Count-1);
        }
        else
        {
            lastId++;
            if (lastId>= CurrentLib.Value.Tracks.Count)
            {
                lastId = 0;
            }
        }

        CurrentTrack.SetState(CurrentLib.Value.Tracks[lastId]);
    }

    public void SaveClicked()
    {
        SavePanel.SetActive(true);
        LibNameInput.text = CurrentLib.Value.Name;
    }

    public void LoadClicked()
    {
        LoadPanel.SetActive(true);

        foreach (Transform t in LibsHub)
        {
            Destroy(t.gameObject);
        }

        foreach (TrackLib lib in SessionsManipulator.Instance.TrackLibs)
        {
            GameObject newBtn =  Instantiate(TrackLibBtnPrefab);
            newBtn.transform.SetParent(LibsHub);
            TrackLib libb = lib;
            newBtn.GetComponent<Button>().onClick.AddListener(() => { CurrentLib.SetState(libb); LoadPanel.SetActive(false); });
            newBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = lib.Name;
            if (CurrentLib.Value == lib)
            {
                newBtn.GetComponent<Button>().interactable = false;
            }
        }
        //fill 
    }

    public void SaveLib()
    {
        CurrentLib.Value.Name = LibNameInput.text;
        SessionsManipulator.Instance.SaveTrackLib(CurrentLib.Value);
    }

    public void TogglePlay()
    {
        Playing.SetState(!Playing.Value);
    }

    public void ToggleShuffle()
    {
        Shufle.SetState(!Shufle.Value);
    }
}
