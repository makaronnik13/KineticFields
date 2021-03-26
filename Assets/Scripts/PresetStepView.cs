using System;
using UnityEngine;
using UnityEngine.UI;

public class PresetStepView: MonoBehaviour
{
    public TrackStep s1, s2;

    private PointTrack track;

    [SerializeField]
    private Image LeftOut, RightOut, LeftIn, RightIn, Center, Up, Down;

    public void Init(TrackStep step1, TrackStep step2, PointTrack track)
    {
        this.track = track;
        if (s1!=null)
        {
            s1.HasKey.RemoveListener(StepKeyChanged);
        }
        if (s2 != null)
        {
            s2.HasKey.RemoveListener(StepKeyChanged);
        }

        s1 = step1;
        s2 = step2;

        step1.HasKey.AddListener(StepKeyChanged);
        step2.HasKey.AddListener(StepKeyChanged);

      
        RightIn.color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
        RightOut.color = track.SubColor;
        LeftIn.color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
        LeftOut.color = track.SubColor;
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
            Up.color = track.SubColor;
           Center.color = new Color(track.MainColor.r, track.MainColor.g, track.MainColor.b, 0.5f);
            Down.color = track.SubColor;
        }
        else
        {
            Up.color = new Color(1, 1, 1, 0f);
            Center.color = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            Down.color = new Color(1, 1, 1, 0f);
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