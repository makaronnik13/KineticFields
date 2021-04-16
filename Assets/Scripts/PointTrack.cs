using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PointTrack
{
    [SerializeField]
    public List<TrackStep> PositionSteps = new List<TrackStep>();

    [SerializeField]
    public List<TrackStep> RadiusSteps = new List<TrackStep>();


    [SerializeField]
    public int presetId;

    [NonSerialized]
    public Action<int> OnTrackChanged = (id) => { };

    [NonSerialized]
    public Action<PointTrack> OnTrackRemoved = (tr) => { };

    public Color SubColor
    {
        get
        {
            return preset.Color;
        }
    }

    public Color MainColor
    {
        get
        {
            return preset.Color2;
        }
    }

    private KineticPreset preset
    {
        get
        {
            if (presetId == -1)
            {
                return KineticFieldController.Instance.Session.Value.MainPreset;
            }
            return KineticFieldController.Instance.Session.Value.Presets[presetId];
        }
    }

    /*
    public Vector3 GetPosition(float time)
    {
        
    }

    public float GetRadius(float time)
    {

    }
    */

    public TrackStep GetStep(float v, bool pos = true)
    {
        //Debug.Log(v+"/"+steps.FirstOrDefault(s => s.Time.Value == v));
        if (pos)
        {
            return PositionSteps.FirstOrDefault(s => Mathf.Abs(s.Time.Value - v) <= 0.01f);
        }

        return RadiusSteps.FirstOrDefault(s=> Mathf.Abs(s.Time.Value - v)<=0.01f);
    }

    public PointTrack(int presetId)
    {
        this.presetId = presetId;
        /*(for (int i = 0; i < 128; i++)
        {
            //PositionSteps.Add(new TrackStep(i/128f, Vector3.zero));
        }*/
    }

    public void AddStep(float t, Vector2 pos)
    {
    
        if (t<0 || t>=PositionSteps.Count)
        {
            return;
        }


        TrackStep step = PositionSteps[Mathf.RoundToInt(t)];



        if (step == null)
        {

            step = new TrackStep(t, pos);
            PositionSteps.Add(step);

 
            OnTrackChanged(presetId);
        }


        step.Position = pos;
    }

    public void RemoveStep(TrackStep s)
    {
        s.Position = Vector3.zero;
        s.HasKey.SetState(false);


        if (PositionSteps.FirstOrDefault(st => st.HasKey.Value) == null)
        {
            OnTrackRemoved(this);
        }
    }

    public bool GetPosition(float v, out Vector2 pos)
    {

        v /= 2f;

        List<TrackStep> avaliableSteps = PositionSteps.Where(s => s.HasKey.Value && Mathf.RoundToInt(s.Time.Value * 64f) <= TracksManager.Instance.CurrentTrack.Value.Steps*2).ToList() ;

        TrackStep s1 = avaliableSteps.OrderByDescending(s=>s.Time.Value).FirstOrDefault(s=>s.Time.Value<=v && s.HasKey.Value);
        TrackStep s2 = avaliableSteps.OrderBy(s => s.Time.Value).FirstOrDefault(s => s.Time.Value >= v && s.HasKey.Value);


        
     

        if (s1 == null)
        {
            s1 = avaliableSteps.OrderByDescending(s=>s.Time.Value).FirstOrDefault();
        }

        if (s2 == null)
        {
            s2 = avaliableSteps.OrderBy(s => s.Time.Value).FirstOrDefault();
        }

        if (s1 == null || s2 == null)
        {
            pos = Vector3.zero;
            return false;
        }

        float dist = 0;
        float val = 0;

        if (s1.Time.Value<s2.Time.Value)
        {
            dist = s2.Time.Value - s1.Time.Value;
            val = v - s1.Time.Value;
        }
        else
        {


            float d = TracksManager.Instance.CurrentTrack.Value.Steps*4f - s1.Time.Value * 128 + s2.Time.Value * 128;
         

            dist = d/64f;

            if (v>s1.Time.Value)
            {
               
                val = (v - s1.Time.Value)*2;
            }
            if (v<s2.Time.Value)
            {
                val =  (dist- s1.Time.Value+v)*2f;
            }
        }

            pos = Vector2.Lerp(s1.Position, s2.Position, val/dist);
            if (s2 == s1)
            {
                pos = s1.Position;
            }
        
 

       

        return true;
    }
}