using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTrackView : MonoBehaviour
{
    [SerializeField]
    private GameObject RepeatPanelPrefab;

    [SerializeField]
    private Transform RepeatPanelsHub;

    [SerializeField]
    private Image Icon, Background;

    [SerializeField]
    private Animator Animator;

 

    public TrackInstance Track;

    public void Init(TrackInstance track)
    {
        Track = track;
        FindObjectOfType<TracksManager>().CurrentTrack.AddListener(CurrentTrackChanged);
        Icon.sprite = track.Icon;
        Background.color = new Color(track.Color.r, track.Color.g, track.Color.b, 0.1f);
        Track.RepeatCount.AddListener(RepeatCountChanged);
        Track.CurrentRepeat.AddListener(CurrentrepeatChanged);
    }

   

    private void CurrentrepeatChanged(int v)
    {
        for (int i = Track.RepeatCount.Value-1; i >=0; i--)
        {
            RepeatPanelsHub.transform.GetChild(i).GetChild(0).gameObject.SetActive(i>=v);
        }
    }

    private void RepeatCountChanged(int v)
    {
        foreach (Transform t in RepeatPanelsHub)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < v; i++)
        {
            GameObject newRepPanel = Instantiate(RepeatPanelPrefab);
            newRepPanel.transform.SetParent(RepeatPanelsHub);
            newRepPanel.transform.localPosition = Vector3.zero;
            newRepPanel.transform.localScale = Vector3.one;
            newRepPanel.GetComponent<Image>().color = Track.Color * 0.3f;
            newRepPanel.transform.GetChild(0).GetComponent<Image>().color = Track.Color;
        }

        Track.CurrentRepeat.SetState(0);
    }

    private void OnDestroy()
    {
        if (FindObjectOfType<TracksManager>()!=null)
        {
            FindObjectOfType<TracksManager>().CurrentTrack.RemoveListener(CurrentTrackChanged);
        }

        Track.RepeatCount.RemoveListener(RepeatCountChanged);
        Track.CurrentRepeat.RemoveListener(CurrentrepeatChanged);
    }

   

    private void CurrentTrackChanged(TrackInstance track)
    {
        Animator.SetBool("Show", track == Track);
    }

   

    public void SelectTrack()
    {
        if (Animator.GetBool("Show"))
        {
            TracksManager.Instance.SetTrack(null);
        }
        else
        {
            TracksManager.Instance.SetTrack(Track);
        }
    }
}
