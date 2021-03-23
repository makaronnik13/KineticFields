using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackView : MonoBehaviour
{
    [SerializeField]
    private Slider Slider;

    [SerializeField]
    private Transform Hub;

    [SerializeField]
    private GameObject Step;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private TracksManager TracksManager;

    private TrackInstance currentTrack;

    private void Start()
    {
        TracksManager.CurrentTrack.AddListener(TrackChanged);
    }

    private void TrackChanged(TrackInstance track)
    {
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
        }

        View.SetActive(track!=null);
    }

    private void SizeChanged(int v)
    {
        UpdateView();
    }

    private void UpdateView()
    {
        foreach (Transform t in Hub)
        {
            Destroy(t.gameObject);
        }

        if (currentTrack != null)
        {
            for (int i = 0; i < Mathf.Pow(2, currentTrack.Size.Value + 1); i++)
            {
                GameObject newStep = Instantiate(Step);
                newStep.transform.SetParent(Hub);
                newStep.transform.localPosition = Vector3.zero;
                newStep.transform.localScale = Vector3.one;
            }

        }
    }
}
