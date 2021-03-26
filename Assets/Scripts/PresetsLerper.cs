using com.armatur.common.flags;
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

    public GenericFlag<KineticPreset> SelectedPreset = new GenericFlag<KineticPreset>("SelectedPreset", null);

    public RectTransform RadiusView;

    public Action<KineticPreset> OnPresetDeleted = (p) => { };

    private int beats = 0;

    public Dictionary<KineticPreset, float> Weigths
    {
        get
        {
            Dictionary<KineticPreset, float> weigths = new Dictionary<KineticPreset, float>();
            foreach (PresetPoint point in points)
            {
                point.Volume.SetState(1f- Vector3.Distance(point.transform.position, RadiusView.transform.position)/Radius.Value);

                if (point.Volume.Value>0)
                {
                    weigths.Add(point.Preset, point.Volume.Value);
                }
     
            } 

            return weigths;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
        Rate.AddListener(RateChanged);
        Radius.AddListener(RadiusChanged);
        SelectedPreset.AddListener(SelectedPresetChanged);
    }

    private void Update()
    {
        if (View.activeInHierarchy && SelectedPreset.Value!=null && KineticFieldController.Instance.Session.Value.Presets.Count>1)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSelected();
            }
        }
    }

    private void DeleteSelected()
    {

        KineticPreset deletedPreset = SelectedPreset.Value;


        OnPresetDeleted(deletedPreset);

        KineticFieldController.Instance.Session.Value.Presets.Remove(deletedPreset);

        PresetPoint point =  points.FirstOrDefault(p=>p.Preset == SelectedPreset.Value);

        points.Remove(point);
        Destroy(point.gameObject);
        SelectedPreset.SetState(null);
    }

    private void SelectedPresetChanged(KineticPreset p)
    {
        SessionsManipulator.Instance.ActivePointChanged(null);
        foreach (PresetPoint pp in points)
        {
            pp.SetSelected(pp.Preset == p);
        }
    }

    public Vector2 GetPosition(KineticPreset preset)
    {
        PresetPoint po = points.FirstOrDefault(p => p.Preset == preset);

        if (po == null)
        {
            return FindObjectOfType<Center>().transform.localPosition;
        }
        return (Vector2)po.transform.localPosition;
    }

    private void RadiusChanged(float v)
    {
        RadiusView.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v*2);
        RadiusView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v*2);
    }

    public void DuplicateSelected()
    {
        KineticPreset duplPreset = SelectedPreset.Value.Clone() as KineticPreset;
        KineticFieldController.Instance.Session.Value.Presets.Add(duplPreset);
        duplPreset.Position = SelectedPreset.Value.Position + Vector2.right * 25f;
        CreatePoint(duplPreset);
        SelectedPreset.SetState(duplPreset);
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
                CreatePoint(preset);
            }
        }
        else
        {
            foreach (PresetPoint p in points)
            {
                Destroy(p.gameObject);
            }
            points.Clear();
            SelectedPreset.SetState(null);
            SessionsManipulator.Instance.ActivePointChanged(null);
        }

        PointsCamera.enabled = !v;
        PresetEditView.SetActive(!v);
    }

    private void CreatePoint(KineticPreset preset)
    {
        GameObject newPoint = Instantiate(PointPrefab);
        newPoint.transform.localScale = Vector3.one;
        newPoint.transform.SetParent(View.transform);
        newPoint.transform.localPosition = new Vector3(preset.X, preset.Y, 0);
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


    public void CreateNewPreset()
    {
        Vector2 localpoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);
        //Vector2 normalizedPoint = Rect.PointToNormalized(GetComponent<RectTransform>().rect, localpoint);

        KineticPreset newPreset = new KineticPreset("Preset_" + KineticFieldController.Instance.Session.Value.Presets.Count);
        newPreset.Position = new Vector2(localpoint.x, localpoint.y);
        KineticFieldController.Instance.Session.Value.Presets.Add(newPreset);
        CreatePoint(newPreset);
        SelectedPreset.SetState(newPreset);
    }
}
