﻿using Assets.Scripts;
using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class KineticFieldController: Singleton<KineticFieldController>
{
    private KeyCode[] keyCodes = 
        {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
         KeyCode.Alpha0
    };


    [SerializeField]
    private List<Sprite> OscilatorSprites;

    [SerializeField]
    public VisualEffect Visual;

    [SerializeField]
    private BarSpectrum SpectrumBar;


    [SerializeField]
    private FrequencyGapEditor GapEditor;


    public GenericFlag<KineticSession> Session = new GenericFlag<KineticSession>("CurrentSession", null);
    public GenericFlag<KineticPoint> ActivePoint = new GenericFlag<KineticPoint>("ActivePoint", null);
    public GenericFlag<FrequencyGap> ActiveGap = new GenericFlag<FrequencyGap>("ActiveGap", null);



    public List<ISource> Sources
    {
        get
        {
            List<ISource> scs = new List<ISource>();
            scs.Add(new FrequencyGap("none", 0, 0 , Color.red, DefaultResources.GapSprites[0]));
            if (Session.Value!=null)
            {
                scs.AddRange(Session.Value.ActivePreset.Value.Gaps);
            }
            //scs.AddRange(Session.ActivePreset.Value.Oscilators);   
            return scs;
        }
    }


    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ActivePoint.Value)
            {
                ActivePoint.Value.Point.Active.SetState(!ActivePoint.Value.Point.Active.Value);
            }
        }
    }

    public void LoadSession(KineticSession session)
    {
        Session.SetState(session);
        LoadPreset(1);
        /*
        int i = 1;
        foreach (AnimationCurve curve in DefaultResources.Settings.SizeCurves)
        {
            GameObject ng = new GameObject("Oscilator_" + i);
            Oscilator osc = ng.AddComponent<Oscilator>();
            osc.Curve = curve;
            osc.icon = OscilatorSprites[i-1];
            Oscilators.Add(osc);
            i++;
        }
        */
    }

    private void SessionChanged(KineticSession session)
    {
        session.ActivePreset.SetState(session.Presets[0]);
    }

    private void ActivePointChanged(KineticPoint obj)
    {
        if (obj!=null)
        {
            ActiveGap.SetState(null);
        }

    }

    private void ActiveGapChanged(FrequencyGap obj)
    {
        if (obj != null)
        {
            ActivePoint.SetState(null);
        }
    }

    private void Start()
    {
        ActivePoint.AddListener(ActivePointChanged);
        ActiveGap.AddListener(ActiveGapChanged);
        Session.AddListener(SessionChanged);
    }

    private void Update()
    {
        foreach (FrequencyGap fg in Session.Value.ActivePreset.Value.Gaps)
        {
            int start = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.Start);
            int end = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.End);
            List<float> data = SpectrumBar.GetSpectrumData().ToList().GetRange(start, end - start);
            float dataAverage = data.Sum()/data.Count;
            fg.UpdateFrequency(dataAverage, data.Max());
        }

        for (int i = 0; i < keyCodes.Count(); i++)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    Session.Value.SavePreset(i);
                }
            }
            else
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    LoadPreset(i);
                }
            }

        }

    }

    public void LoadPreset(int i)
    {

        ActivePoint.SetState(null);
        ActiveGap.SetState(null);

        if (Session.Value.ActivePreset.Value!=null)
        {

        }

        Session.Value.LoadPreset(i);
        GapEditor.Init(Session.Value.ActivePreset.Value);

        int j = 0;
        foreach (KineticPoint kp in FindObjectsOfType<KineticPoint>().OrderBy(kp=>kp.transform.GetSiblingIndex()))
        {
            kp.Init(Session.Value.Presets[i].Points[j]);
            j++;
        }
    }

}
