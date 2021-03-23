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

    //[SerializeField]
    //private Animator PlayStopBtnAnimator;

    [SerializeField]
    private Image PlayStopIcon;

    [SerializeField]
    private Sprite PlaySprte, StopSprite;

    [SerializeField]
    private TMPro.TMP_InputField LibNameInput;

    [SerializeField]
    private Transform LibsHub;

    [SerializeField]
    private GameObject PlayStopBtn;

    //[SerializeField]
    //private TMPro.TextMeshProUGUI ChangeRate;

    public GenericFlag<TrackLib> CurrentLib = new GenericFlag<TrackLib>("TrackLib", null);

    public GenericFlag<TrackInstance> CurrentTrack = new GenericFlag<TrackInstance>("CurrentTrack", null);

    private TrackLib _lastLib;

    private void Awake()
    {
        if (CurrentLib.Value == null)
        {
            CurrentLib.SetState(new TrackLib("NewTrackLib"));
        }
        CurrentLib.AddListener(LibChanged);
    }

    private void LibChanged(TrackLib lib)
    {

        if (lib.Tracks.Count>0)
        {
            CurrentTrack.SetState(lib.Tracks[0]);
        }
        else
        {
            CurrentTrack.SetState(null);
        }

        if (_lastLib!=lib)
        {
            if (_lastLib!=null)
            {

            }
            _lastLib = lib;

            _lastLib.ChangeRate.AddListener(LibRateChanged);
            _lastLib.Playing.AddListener(LibPlayingStateChanged);
        }

        PlayStopBtn.SetActive(CurrentLib.Value.Tracks.Count>0);
    }

    private void LibRateChanged(int v)
    {
       
    }

    private void LibPlayingStateChanged(bool v)
    {
        //PlayStopBtnAnimator.SetBool("Show", v);
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
        
    }

    public void DeleteCurrentTrack()
    {
        CurrentLib.Value.Tracks.Remove(CurrentTrack.Value);
        Destroy(GetComponentsInChildren<SingleTrackView>().FirstOrDefault(v=>v.Track == CurrentTrack.Value).gameObject);
        LibChanged(CurrentLib.Value);
    }

    public void CreateTrack()
    {
        int id = UnityEngine.Random.Range(0, DefaultResources.TrackSprites.Count - 1);
        TrackInstance newTrack = new TrackInstance(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f), DefaultResources.TrackSprites[id]);

        CreateTrackBtn(newTrack);

        CurrentLib.Value.Tracks.Add(newTrack);

        LibChanged(CurrentLib.Value);

        CurrentTrack.SetState(newTrack);
    }

    private void CreateTrackBtn(TrackInstance newTrack)
    {
        GameObject newTrackBtn = Instantiate(TrackBtnPrefab);
        newTrackBtn.transform.SetParent(transform);
        newTrackBtn.transform.localPosition = Vector3.zero;
        newTrackBtn.transform.localScale = Vector3.one;
        newTrackBtn.GetComponent<SingleTrackView>().Init(newTrack);
        newTrackBtn.transform.SetSiblingIndex(CurrentLib.Value.Tracks.Count+2);
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
            newBtn.GetComponent<Button>().onClick.AddListener(() => { CurrentLib.SetState(libb); });
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
        CurrentLib.Value.Playing.SetState(!CurrentLib.Value.Playing.Value);
    }
}
