using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetPathView : Singleton<PresetPathView>
{
    private BezierSpline Spline;

    [SerializeField]
    public GameObject HandlePrefab;

    [SerializeField]
    public LineRenderer LineRenderer;

    private List<GameObject> Handles = new List<GameObject>();

    private void Start()
    {
        Spline.onSplineChanged += UpdateView;
    }

    public void Init(PresetPath path)
    {
        Destroy(Spline);
        Spline = gameObject.AddComponent<BezierSpline>();
        Spline.Initialize(path.Points.Count);

        for (int i = 0; i < path.Points.Count; i++)
        {
            Spline[0].localPosition = path.Points[i].Vector3;
        }

        Spline.Refresh();

        UpdateHandles();
    }

    private void UpdateHandles()
    {
        foreach (GameObject handle in Handles)
        {
            Destroy(handle);
        }

        foreach (Vector3 pos in Spline.pointCache.positions)
        {
            GameObject newHandle = Instantiate(HandlePrefab);
            newHandle.transform.SetParent(transform);
            newHandle.transform.position = pos;
        }
    }

    private void UpdateView(BezierSpline spline, DirtyFlags dirtyFlags)
    {
        Debug.Log("update view");
        
        LineRenderer.positionCount = 100;
        for (int i = 0; i < 100; i++)
        {
            LineRenderer.SetPosition(i, spline.GetPoint(i/100f));
        }

        for (int i = 0; i < Spline.pointCache.positions.Length; i++)
        {
        //    Handles[i].transform.position = Spline.pointCache.positions[i];
        }
      
    }

    private void SetVivible(bool v)
    {
        LineRenderer.enabled = v;
    }

}
