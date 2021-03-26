using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTrackView : MonoBehaviour
{
    [SerializeField]
    private Image Icon, Background;

    [SerializeField]
    private TMPro.TextMeshProUGUI Length;

    [SerializeField]
    private Animator Animator;

    public TrackInstance Track;

    public void Init(TrackInstance track)
    {
        Track = track;
        Track.Size.AddListener(SizeChanged);
        FindObjectOfType<TracksManager>().CurrentTrack.AddListener(CurrentTrackChanged);
        Icon.sprite = track.Icon;
        Background.color = new Color(track.Color.r, track.Color.g, track.Color.b, 0.1f);
    }

    private void OnDestroy()
    {
        Debug.Log("destr "+name);
        if (FindObjectOfType<TracksManager>()!=null)
        {
            FindObjectOfType<TracksManager>().CurrentTrack.RemoveListener(CurrentTrackChanged);
        }
       
    }

    private void SizeChanged(int v)
    {
        Length.text = Mathf.Pow(2, 1 + v).ToString();
    }

    private void CurrentTrackChanged(TrackInstance track)
    {


        Animator.SetBool("Show", track == Track);
    }

    public void SizeClicked()
    {
        Track.Size.SetState(Track.Size.Value+1);
        if (Track.Size.Value>3)
        {
            Track.Size.SetState(0);
        }
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
