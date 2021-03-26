﻿using Assets.Scripts;
using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class KineticFieldController : Singleton<KineticFieldController>
{
    private KeyCode[] keyCodes =
    {
         KeyCode.Alpha0,
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9
    };

    [SerializeField]
    private Transform OscilatorsHub;

    [SerializeField]
    public VisualEffect Visual;

    [SerializeField]
    private BarSpectrum SpectrumBar;

    [SerializeField]
    private FrequencyGapEditor GapEditor;

    [SerializeField]
    private float UpdateAveragePresetStep = 0.1f;

    public GenericFlag<KineticSession> Session = new GenericFlag<KineticSession>("CurrentSession", null);
    public GenericFlag<KineticPoint> ActivePoint = new GenericFlag<KineticPoint>("ActivePoint", null);
    public GenericFlag<FrequencyGap> ActiveGap = new GenericFlag<FrequencyGap>("ActiveGap", null);
    public GenericFlag<ISource> DraggingSource = new GenericFlag<ISource>("DraggingSource", null);

    public GenericFlag<ISource> SelectedSource = new GenericFlag<ISource>("SelectedSource", null);

    public bool KeysEnabled = true;

    [SerializeField]
    private GameObject draggingSourceView;

    private KineticPointInstance CoppyingPoint;

    public List<ISource> Sources
    {
        get
        {
            List<ISource> scs = new List<ISource>();
            scs.Add(new FrequencyGap("none", 0, 0, Color.red, DefaultResources.GapSprites[0]));
            if (Session.Value != null)
            {
                scs.AddRange(Session.Value.Gaps);
                scs.AddRange(Session.Value.Oscilators);
            }


            return scs;
        }
    }


    private void FixedUpdate()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ActivePoint.Value)
            {
                ActivePoint.Value.Point.Active.SetState(!ActivePoint.Value.Point.Active.Value);
            }
        }*/
    }

    public void LoadSession(KineticSession session)
    {

        Session.SetState(session);
        LoadPreset(session.Presets.FirstOrDefault());
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


        int i = 0;
        foreach (OscilatorView oscView in OscilatorsHub.GetComponentsInChildren<OscilatorView>())
        {
            oscView.Init(session.Oscilators[i]);
            i++;
        }

        PresetsLerper.Instance.SelectedPreset.AddListener(LoadPreset);
        session.LoadPreset(session.Presets.FirstOrDefault());
    }

    private void ActivePointChanged(KineticPoint obj)
    {
        if (obj != null)
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
        DraggingSource.AddListener(DraggingSourceChanged);
        StartCoroutine(UpdateAveragePreset());
    }

    private IEnumerator UpdateAveragePreset()
    {
        while (true)
        {
            yield return new WaitForSeconds(UpdateAveragePresetStep);
            if (!PresetsLerper.Instance.View.activeInHierarchy)
            {
                UpdateVisual(Session.Value.ActivePreset.Value);
            }
            else
            {
                Session.Value.UpdateAveragePreset(PresetsLerper.Instance.Weigths);
                UpdateVisual(Session.Value.AveragePreset, true);
            }
        }
    }

    private void DraggingSourceChanged(ISource source)
    {

        draggingSourceView.SetActive(source != null);

        if (source != null)
        {
            draggingSourceView.transform.GetChild(0).GetComponent<Image>().sprite = source.Icon;
        }
    }

    private void Update()
    {
        draggingSourceView.transform.position = Input.mousePosition;
        if (DraggingSource.Value != null && Input.GetMouseButtonUp(0))
        {
            DraggingSource.SetState(null);
        }

        List<float> d = SpectrumBar.GetSpectrumData().ToList();
        foreach (FrequencyGap fg in Session.Value.Gaps)
        {
            int start = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.Start);
            int end = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.End);
            List<float> data = d.GetRange(start, end - start);
            float dataAverage = data.Sum() / data.Count;
            fg.UpdateFrequency(data);
        }

        if (ActivePoint.Value)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    CoppyingPoint = ActivePoint.Value.Point;
                }
                if (CoppyingPoint != null && Input.GetKeyDown(KeyCode.V))
                {
                    ActivePoint.Value.Point.Active.SetState(CoppyingPoint.Active.Value);
                    ActivePoint.Value.Point.Curve = CoppyingPoint.Curve;
                    ActivePoint.Value.Point.Deep = CoppyingPoint.Deep.Clone() as ModifyingParameter;
                    ActivePoint.Value.Point.Gradient = CoppyingPoint.Gradient;
                    ActivePoint.Value.Point.Position = CoppyingPoint.Position;
                    ActivePoint.Value.Point.Radius = CoppyingPoint.Radius.Clone() as ModifyingParameter;
                    ActivePoint.Value.Point.Volume = CoppyingPoint.Volume.Clone() as ModifyingParameter;
                }
            }
        }
        /*
                if (KeysEnabled && EventSystem.current.currentSelectedGameObject == null)
                {
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
        */


        if (Input.GetMouseButtonDown(1))
        {
            ActivePoint.SetState(null);
        }

    }

    private void UpdateVisual(KineticPreset preset, bool useTemp = false)
    {

        Visual.SetFloat("FrontCutPlane", preset.NearCutPlane.Value.Value);
        Visual.SetFloat("BackCutPlane", preset.FarCutPlane.Value.Value);
        Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[preset.MeshId.Value]);
        Visual.SetFloat("Lifetime", preset.Lifetime.Value.Value);
        Visual.SetInt("Rate", Mathf.RoundToInt(preset.ParticlesCount.Value.Value));


        Visual.SetFloat("Size", (0.05f + preset.MainPoint.Deep.Value.Value - 1f) / 8f);
        //size

        for (int i = 0; i < 13; i++)
        {
            if (preset.Points.FirstOrDefault(p => p.Id == i).Active.Value || useTemp)
            {

                Visual.SetFloat("P" + i + "Radius", preset.Points.FirstOrDefault(p => p.Id == i).Radius.Value.Value);
                Visual.SetFloat("P" + i + "Value", preset.Points.FirstOrDefault(p => p.Id == i).Volume.Value.Value);
            }
            else
            {

                Visual.SetFloat("P" + i + "Radius", 0);
                Visual.SetFloat("P" + i + "Value", 0);
            }

            if (useTemp)
            {
                if (preset.Points.FirstOrDefault(p => p.Id == i).ShowGradient)
                {
                    Visual.SetGradient("P" + i + "Gradient".ToString(), preset.Points.FirstOrDefault(p => p.Id == i).TempGradient.Gradient);
                    Visual.SetAnimationCurve("P" + i + "Func", preset.Points.FirstOrDefault(p => p.Id == i).TempCurve.Curve);
                }

            }
            else
            {
                Visual.SetGradient("P" + i + "Gradient".ToString(), preset.Points.FirstOrDefault(p => p.Id == i).Gradient.Gradient);
                Visual.SetAnimationCurve("P" + i + "Func", preset.Points.FirstOrDefault(p => p.Id == i).Curve.Curve);
            }
        }

    }

    public void LoadPreset(KineticPreset preset)
    {

        ActivePoint.SetState(null);
        ActiveGap.SetState(null);

        if (Session.Value.ActivePreset.Value != null)
        {

        }

        Session.Value.LoadPreset(preset);
        GapEditor.Init(Session.Value);

        int j = 0;
        foreach (KineticPoint kp in FindObjectsOfType<KineticPoint>().OrderBy(kp => kp.transform.GetSiblingIndex()))
        {
            kp.Init(Session.Value.ActivePreset.Value.Points.FirstOrDefault(p => p.Id == j));
            j++;
        }
    }

    public void SyncOsc()
    {
        foreach (Oscilator o in Session.Value.Oscilators)
        {
            o.Reset();
        }
    }

}
