using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetsLerper : MonoBehaviour
{
    private KineticSession Session;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private GameObject PointPrefab;

    private List<PresetPoint> points = new List<PresetPoint>();

    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
    }

    private void SessionChanged(KineticSession session)
    {
        this.Session = session;
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
}
