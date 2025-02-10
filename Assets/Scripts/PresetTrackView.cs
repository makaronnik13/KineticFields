using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PresetTrackView : MonoBehaviour
{
    [SerializeField]
    private GameObject StepPrefab;

    [SerializeField]
    private RectTransform LinesHub;

    private PointTrack Track;

    private List<GameObject> lines = new List<GameObject>();

    public void Init(PointTrack track)
    {
        if (Track!=null)
        {
            Track.OnTrackChanged -= UpdateTrack;
        }
        Track = track;
        Track.OnTrackChanged += UpdateTrack;
        UpdateTrack();
    }

    private void OnDestroy()
    {
        Track.OnTrackChanged -= UpdateTrack;
    }

    public void UpdateTrack()
    {
        if (TracksManager.Instance.CurrentTrack.Value == null)
        {
            return;
        }

        float stepSize = 16f*LinesHub.rect.width / TracksManager.Instance.CurrentTrack.Value.Steps;



        foreach (GameObject line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();

        float lastStartTime = 0;
        float lastTime = 0;
        RectTransform lastLine = null;

        float oneStep = 2f / 128f;

        List<TrackStep> avaliableSteps = Track.Steps.Where(s => Mathf.FloorToInt(s.Time.Value * 64f) <= TracksManager.Instance.CurrentTrack.Value.Steps * 4f).ToList();

        foreach (TrackStep step in avaliableSteps.OrderBy(s=>s.Time.Value))
        {

   
            if (lastTime == 0 ||  step.Time.Value-lastTime > oneStep/4f)
            {
                GameObject newLine = Instantiate(StepPrefab);
                lastLine = newLine.GetComponent<RectTransform>();
                lastLine.SetParent(LinesHub);
                lastStartTime = step.Time.Value;
                lines.Add(newLine);
            }

            lastTime = step.Time.Value + oneStep/4f;
            lastLine.pivot = new Vector2(0, 0.5f);
            lastLine.anchorMin = new Vector2(0,0f);
            lastLine.anchorMax = new Vector2(0,1f);



            lastLine.anchoredPosition = new Vector2((lastStartTime-oneStep) * stepSize, 0);
            lastLine.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,(lastTime - lastStartTime) * stepSize);
            lastLine.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 7f);
        }
    }

    public void RemoveStep(float pos)
    {
        float posFull = pos/ LinesHub.GetComponent<RectTransform>().rect.width;

        if (pos == 0)
        {
            posFull = 0;
        }

 
        float oneStep = 1f / 64f; 
        float coef = 2f*TracksManager.Instance.CurrentTrack.Value.Steps / 32f;

        float time = coef * posFull+oneStep;

        TrackStep removingStep = Track.Steps.FirstOrDefault(s => Mathf.Abs(time - s.Time.Value)<oneStep/4f);

        if (removingStep != null)
        {
            Track.RemoveStep(removingStep);

            UpdateTrack();
        }
    }
}
