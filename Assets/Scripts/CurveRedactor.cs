using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurveRedactor : Singleton<CurveRedactor>
{
    private Coroutine doubleClickCoroutine;

    public CurveInstance Curve;
    public TMPro.TMP_InputField TimeInput, ValueInput;

    [SerializeField]
    private GameObject CurrentHandleView;
    [SerializeField]
    private Transform HandlesHub;

    private AnimationCurve editingCurve;
    private int samples = 50;
    private float horizontalMultiplyer = 0.5f;

    public GameObject HandlePrefab;

    public GenericFlag<CurveHandle> EditingPoint = new GenericFlag<CurveHandle>("EditingPoint", null);

    public List<CurveHandle> handles = new List<CurveHandle>();

    [SerializeField]
    private LineRenderer Line3d;
    public GameObject View;

    private void Start()
    {
        ValueInput.onEndEdit.AddListener(ValueChanged);
        TimeInput.onEndEdit.AddListener(TimeChanged);
        EditingPoint.AddListener(CurrentHandleChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && EditingPoint.Value)
        {
            int index = handles.IndexOf(EditingPoint.Value);
            if (index!=0 && index!=handles.Count-1)
            {
                editingCurve.RemoveKey(index);
                handles.Remove(EditingPoint.Value);
                Destroy(EditingPoint.Value.gameObject);
                EditingPoint.SetState(null);
                HandleChanged(null);
            }
        }
    }

    private void CurrentHandleChanged(CurveHandle handle)
    {

        CurrentHandleView.SetActive(handle!=null);

        if (handle != null)
        {
            TimeInput.text = handle.kf.time.ToString();
            ValueInput.text = handle.kf.value.ToString();

            TimeInput.interactable = (handles.IndexOf(handle) != 0) && (handles.IndexOf(handle) != handles.Count - 1);
           
        }

        
    }

    private void TimeChanged(string v)
    {
        EditingPoint.Value.SetTime(float.Parse(v));
    }

    private void ValueChanged(string v)
    {
        EditingPoint.Value.SetValue(float.Parse(v));
    }

    public void Apply()
    {
        Debug.Log(editingCurve.keys.Length);
        Curve.Curve = new AnimationCurve(editingCurve.keys);
        Debug.Log(Curve.Curve.keys.Length);
        Curve.OnEdited();
        Cancel();
    }

    public void Cancel()
    {
        foreach (CurveHandle ch in handles)
        {
            Destroy(ch.gameObject);
        }
        View.SetActive(false);
        handles.Clear();
    }

    [ContextMenu("test")]
    public void Test()
    {
        EditCurve(Curve);
    }

    public void EditCurve(CurveInstance curve)
    {
        Curve = curve;
        editingCurve = new AnimationCurve(Curve.Curve.keys);
        View.SetActive(true);

        Debug.Log(editingCurve.keys.Length);
        foreach (Keyframe kf in editingCurve.keys)
        {
            CreateKey(kf);
        }
        UpdateCurve();
    }

    private CurveHandle CreateKey(Keyframe kf)
    {
        GameObject newHandle = Instantiate(HandlePrefab);
        newHandle.transform.SetParent(HandlesHub);
        newHandle.GetComponent<RectTransform>().anchoredPosition = new Vector2(kf.time*300f, 100*kf.value);
        handles.Add(newHandle.GetComponent<CurveHandle>());
        handles = handles.OrderBy(h=>h.kf.time).ToList();
        newHandle.GetComponent<CurveHandle>().Init(editingCurve.keys.ToList().IndexOf(kf), kf, HandleChanged);
        return newHandle.GetComponent<CurveHandle>();
    }

    private void HandleChanged(CurveHandle ch)
    {
        if (ch == EditingPoint.Value)
        {
            CurrentHandleChanged(ch);
        }

        handles = handles.OrderBy(h => h.kf.time).ToList();

        for (int i = 0;i<handles.Count;i++)
        {
            Debug.Log(handles.Count+"/"+editingCurve.keys.Length);

            editingCurve.MoveKey(i, handles[i].kf);
        }

        UpdateCurve();
    }

    private void UpdateCurve()
    {
        Line3d.positionCount = samples;

        for (int i = 0; i < samples; i++)
        {
            float hv = (i + 0.0f) / samples;
            Line3d.SetPosition(i, new Vector3(hv/horizontalMultiplyer, editingCurve.Evaluate(hv),0));
        }
    }

    public void DeselectHandle()
    {
        if (doubleClickCoroutine == null)
        {
            doubleClickCoroutine = StartCoroutine(DoubleClick());
        }
        else
        {
            var v3 = Input.mousePosition;
            v3.z = 10f;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            v3 = HandlesHub.InverseTransformPoint(Camera.main.ScreenToWorldPoint(v3));
            CurveHandle newHandle = CreateKey(new Keyframe(0,0));
            newHandle.transform.position = Input.mousePosition;
            RectTransform rt = newHandle.GetComponent<RectTransform>();
            Keyframe kf = new Keyframe(rt.anchoredPosition.x / 300f, rt.anchoredPosition.y / 100f);
            editingCurve.AddKey(kf);
            newHandle.SetPosition(Input.mousePosition);
        }

        EditingPoint.SetState(null);
    }

    private IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(0.3f);
        doubleClickCoroutine = null;
    }
}
