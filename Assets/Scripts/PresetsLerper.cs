﻿using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetsLerper : Singleton<PresetsLerper>
{
    [SerializeField]
    private GameObject PresetEditView;

    [SerializeField]
    private Camera PointsCamera;

    [SerializeField]
    private TMPro.TextMeshProUGUI RateLable;

    private KineticSession Session;

    [SerializeField]
    public GameObject View;

    [SerializeField]
    private GameObject PointPrefab;

    private List<PresetPoint> points = new List<PresetPoint>();

    private GenericFlag<int> Rate = new GenericFlag<int>("RandomRate", 0);

    public GenericFlag<float> Radius = new GenericFlag<float>("Radius", 250);

    public RectTransform RadiusView;

    private int beats = 0;

    public Dictionary<KineticPreset, float> Weigths
    {
        get
        {
            Dictionary<KineticPreset, float> weigths = new Dictionary<KineticPreset, float>();
            foreach (PresetPoint point in points.Where(p=>Vector3.Distance(p.transform.position, RadiusView.transform.position)<=Radius.Value))
            {
                point.Volume.SetState(1f- Vector3.Distance(point.transform.position, RadiusView.transform.position)/Radius.Value);
                weigths.Add(point.Preset, point.Volume.Value);
            }

            Debug.Log(weigths.Count);
 

            return weigths;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
        Rate.AddListener(RateChanged);
        Radius.AddListener(RadiusChanged);
    }

    private void RadiusChanged(float v)
    {
        RadiusView.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v*2);
        RadiusView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v*2);
    }

    private void RateChanged(int v)
    {
        if (v==0)
        {
            RateLable.text = "-";
        }
        else
        {
            RateLable.text = Mathf.Pow(2, v - 1).ToString();
        }
        beats = 0;

        BpmManager.Instance.OnBeat -= Beat;

        if (v!=0)
        {
            BpmManager.Instance.OnBeat += Beat;
        }
    }

    private void Beat()
    {
        beats++;
        if (beats>= Mathf.Pow(2, Rate.Value-1))
        {
            beats = 0;
            RandomPositions();
        }
    }

    private void SessionChanged(KineticSession session)
    {
        this.Session = session;
    }

    public void Toggle()
    {
        SetState(!View.activeInHierarchy);
    }

    public void SetState(bool v)
    {
        View.SetActive(v);
        if (v)
        {
            int i = 0;

            foreach (KineticPreset preset in KineticFieldController.Instance.Session.Value.Presets)
            {
                CreatePoint(preset, 360f/ KineticFieldController.Instance.Session.Value.Presets.Count*i);
            }
        }
        else
        {
            foreach (PresetPoint p in points)
            {
                Destroy(p.gameObject);
            }
            points.Clear();
        }

        PointsCamera.enabled = !v;
        PresetEditView.SetActive(!v);
    }

    private void CreatePoint(KineticPreset preset, float angle)
    {
        GameObject newPoint = Instantiate(PointPrefab);
        newPoint.transform.localScale = Vector3.one;
        newPoint.transform.SetParent(View.transform);
        newPoint.transform.localPosition = new Vector3(120f*Mathf.Cos(angle*Mathf.Rad2Deg) , 120f * Mathf.Sin(angle * Mathf.Rad2Deg), 0);
        PresetPoint point = newPoint.GetComponent<PresetPoint>();
        point.Init(preset);
        points.Add(point);
    }

    [ContextMenu("Random")]
    public void RandomPositions()
    {
        float angle = 360f / points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            float a = angle * i;

            float r = UnityEngine.Random.Range(0, 150f);

            points[i].transform.localPosition = new Vector3(r * Mathf.Cos(a * Mathf.Rad2Deg), r * Mathf.Sin(a * Mathf.Rad2Deg), 0);
        }
    }

    public void ChangeRate()
    {
        Rate.SetState(Rate.Value+1);
        if (Rate.Value>4)
        {
            Rate.SetState(0);
        }
    }
}
