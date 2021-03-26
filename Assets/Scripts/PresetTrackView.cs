﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetTrackView : MonoBehaviour
{
    [SerializeField]
    public GameObject StepPrefab;

    public PointTrack Track;

    private List<PresetStepView> steps = new List<PresetStepView>();

    private void Start()
    {
        for (int i = 0; i < 128 - 1; i++)
        {
            GameObject ns = PoolManager.Instance.spawnObject(StepPrefab);
            ns.transform.SetParent(transform);
            ns.transform.localPosition = Vector3.zero;
            ns.transform.localScale = Vector3.one;

            PresetStepView view = ns.GetComponent<PresetStepView>();
            

            steps.Add(view);
        }
    }

    public void Init(PointTrack track)
    {
        Track = track;
        //Track.OnTrackChanged += UpdateTrack;

        int i = 0;
        foreach (PresetStepView view in steps)
        {
            view.Init(track.steps[i], track.steps[i + 1], track);
            i++;
        }
        UpdateTrack();
    }

    
    public void UpdateTrack()
    {
        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].gameObject.SetActive(i < TracksManager.Instance.CurrentTrack.Value.Steps*4);
        }

        /*
        Debug.Log("UPD");
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

       
        foreach (TrackStep step in Track.steps)
        {
            Debug.Log(step.Time.Value);
        }

        Debug.Log("_________________");

        for (int i = 0; i < TracksManager.Instance.CurrentTrack.Value.Steps; i++)
        {
            GameObject ns = Instantiate(StepPrefab);
            ns.transform.SetParent(transform);
            ns.transform.localPosition = Vector3.zero;
            ns.transform.localScale = Vector3.one;

            
            
        }
        */
    }
    
}
