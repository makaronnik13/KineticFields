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

    [NonSerialized]
    public Action OnTrackChanged = () => { };

    public enum TrackType
    {
        Position,
        Radius
    }

    private TrackType TType = TrackType.Position;

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
            return KineticFieldController.Instance.Session.Value.MainPreset;
        }
    }

    public List<TrackStep> Steps
    {
        get
        {
            if (TType == TrackType.Radius)
            {
                return RadiusSteps;
            }
            return PositionSteps;
        }
    }

    public TrackStep GetStep(float v, bool pos = true)
    {
        if (pos)
        {
            return PositionSteps.FirstOrDefault(s => Mathf.Abs(s.Time.Value - v) <= 0.01f);
        }

        return RadiusSteps.FirstOrDefault(s=> Mathf.Abs(s.Time.Value - v)<=0.01f);
    }

    public PointTrack(TrackType tType)
    {
        TType = tType;
    }

    public void AddStep(float t, float radius)
    {
        if (t<0)
        {
            return;
        }

        TrackStep step = RadiusSteps.FirstOrDefault(s => Mathf.Abs(s.Time.Value - t) <= 1f / 256f);

        if (step == null)
        {
            step = new TrackStep(t, radius);
            RadiusSteps.Add(step);
        }

        step.Radius = radius;
        OnTrackChanged();
    }

    public void AddStep(float t, Vector2 pos)
    {

        if (t < 0)
        {
            return;
        }

        TrackStep step = PositionSteps.FirstOrDefault(s => Mathf.Abs(s.Time.Value - t)<= 1f/256f);

        if (step == null)
        {

            step = new TrackStep(t, pos);
            PositionSteps.Add(step);
        }

        step.Position = pos;
        OnTrackChanged();
    }

    public void RemoveStep(TrackStep s)
    {
        if (RadiusSteps.Contains(s))
        {
            RadiusSteps.Remove(s);
        }

        if (PositionSteps.Contains(s))
        {
            PositionSteps.Remove(s);
        }

        OnTrackChanged();
    }


    public bool GetRadius(float v, out float radius)
    {
        v += 2f / 128f;

        List<TrackStep> avaliableSteps = RadiusSteps.Where(s => Mathf.FloorToInt(s.Time.Value * 64f) <= TracksManager.Instance.CurrentTrack.Value.Steps * 4f).ToList();

        TrackStep s1 = avaliableSteps.OrderBy(s => s.Time.Value).LastOrDefault(s => s.Time.Value <= v);
        TrackStep s2 = avaliableSteps.OrderBy(s => s.Time.Value).FirstOrDefault(s => s.Time.Value >= v);

        if (s1 == null)
        {
            s1 = avaliableSteps.OrderByDescending(s => s.Time.Value).FirstOrDefault();
        }


        if (s2 == null)
        {

            s2 = avaliableSteps.OrderBy(s => s.Time.Value).FirstOrDefault();
        }


        if (s1 == null || s2 == null)
        {
            radius = 0;
            return false;
        }

        float dist = 0;
        float val = 0;

        if (s1.Time.Value < s2.Time.Value)
        {
            dist = s2.Time.Value - s1.Time.Value;
            val = v - s1.Time.Value;
        }
        else
        {
            dist = 2f * (TracksManager.Instance.CurrentTrack.Value.Steps / 32f) - s1.Time.Value + s2.Time.Value;


            if (v > s1.Time.Value)
            {

                val = v - s1.Time.Value;
            }
            if (v < s2.Time.Value)
            {
                val = 2f * (TracksManager.Instance.CurrentTrack.Value.Steps / 32f) - s1.Time.Value + v;
            }
        }

        radius = Mathf.Lerp(s1.Radius, s2.Radius, val / dist);
        if (s2 == s1)
        {
            radius = s1.Radius;
        }

        return true;
    }

    public bool GetPosition(float v, out Vector2 pos)
    {

        v += 2f / 128f;

        List<TrackStep> avaliableSteps = PositionSteps.Where(s => Mathf.FloorToInt(s.Time.Value * 64f) <= TracksManager.Instance.CurrentTrack.Value.Steps*4f).ToList();

        TrackStep s1 = avaliableSteps.OrderBy(s=>s.Time.Value).LastOrDefault(s=>s.Time.Value<=v);
        TrackStep s2 = avaliableSteps.OrderBy(s => s.Time.Value).FirstOrDefault(s => s.Time.Value >= v);

     

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
            dist = 2f* (TracksManager.Instance.CurrentTrack.Value.Steps/32f) - s1.Time.Value + s2.Time.Value;


            if (v>s1.Time.Value)
            {
               
                val = v - s1.Time.Value;
            }
            if (v<s2.Time.Value)
            {
                val =  2f * (TracksManager.Instance.CurrentTrack.Value.Steps / 32f) - s1.Time.Value + v;
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