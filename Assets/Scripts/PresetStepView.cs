using System;
using UnityEngine;
using UnityEngine.UI;

public class PresetStepView: MonoBehaviour
{
    public TrackStep s1, s2;

    private PointTrack track;

    public void Init(TrackStep step1, TrackStep step2, PointTrack track)
    {
        this.track = track;
        s1 = step1;
        s2 = step2;

        step1.HasKey.AddListener(StepKeyChanged);
        step2.HasKey.AddListener(StepKeyChanged);

        GameObject left = transform.GetChild(0).gameObject;
        GameObject right = transform.GetChild(2).gameObject;

        right.GetComponent<Image>().color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
        right.transform.GetChild(0).GetComponent<Image>().color = track.SubColor;
        left.GetComponent<Image>().color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
        left.transform.GetChild(0).GetComponent<Image>().color = track.SubColor;
        UpdateView();
    }

    private void StepKeyChanged(bool obj)
    {
        UpdateView();
    }

    public void UpdateView()
    {
        
        GameObject left = transform.GetChild(0).gameObject;
        GameObject right = transform.GetChild(2).gameObject;

  
        left.SetActive(s1.HasKey.Value);
        right.SetActive(s2.HasKey.Value);



        RectTransform rt = (transform.GetChild(1) as RectTransform);

        Vector2 minOffset = Vector2.zero;
        Vector2 maxOffset = Vector2.zero;



        if (right.activeInHierarchy)
        {
            

            maxOffset = new Vector2(-5, 0);
        }

        if (left.activeInHierarchy)
        {
            

            minOffset = new Vector2(5, 0);
        }

        rt.offsetMin = minOffset;
        rt.offsetMax = maxOffset;

        if (s1.HasKey.Value && s2.HasKey.Value)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = track.SubColor;
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = track.SubColor;
        }
        else
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0f);
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0f);
        }
    }

    public void TryRemove(int id)
    {
        if (TrackView.Instance.Sliding)
        {
            return;
        }

        if (TrackView.Instance.DraggingPresets.Count==0 && Input.GetMouseButton(0))
        {
            switch (id)
            {
                case 0:
                    track.RemoveStep(s1);
                    break;
                case 1:
                    track.RemoveStep(s1);
                    track.RemoveStep(s2);
                    break;
                case 2:
                    track.RemoveStep(s2);
                    break;
            }
        }
        
    }
}