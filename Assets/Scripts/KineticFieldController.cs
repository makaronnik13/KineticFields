using Assets.Scripts;
using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class KineticFieldController: Singleton<KineticFieldController>
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


    public GenericFlag<KineticSession> Session = new GenericFlag<KineticSession>("CurrentSession", null);
    public GenericFlag<KineticPoint> ActivePoint = new GenericFlag<KineticPoint>("ActivePoint", null);
    public GenericFlag<FrequencyGap> ActiveGap = new GenericFlag<FrequencyGap>("ActiveGap", null);
    public GenericFlag<ISource> DraggingSource = new GenericFlag<ISource>("DraggingSource", null);

    public bool KeysEnabled = true;

    [SerializeField]
    private GameObject draggingSourceView;

    public List<ISource> Sources
    {
        get
        {
            List<ISource> scs = new List<ISource>();
            scs.Add(new FrequencyGap("none", 0, 0 , Color.red, DefaultResources.GapSprites[0]));
            if (Session.Value!=null)
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
        if (Session.Value!=null)
        {
            Session.Value.SelectedPresetPos.RemoveListener(LoadPreset);
        }

        Session.SetState(session);
        LoadPreset(new SerializedVector2Int(0,0));
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
        session.LoadPreset(new SerializedVector2Int(0, 0));

        int i = 0;
        foreach (OscilatorView oscView in OscilatorsHub.GetComponentsInChildren<OscilatorView>())
        {
            oscView.Init(session.Oscilators[i]);
            i++;
        }

        session.SelectedPresetPos.AddListener(LoadPreset);
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

    public void RandomSwap()
    {
        SerializedVector2Int pos = Session.Value.SelectedPresetPos.Value;

        List<SerializedVector2Int> nearIds = new List<SerializedVector2Int>();


        string s = "";


        if (pos.y < Session.Value.PresetsGrid.GetLength(1) - 1)
        {
            SerializedVector2Int p4 = new SerializedVector2Int(pos.x, pos.y + 1);
            if (Session.Value.PresetsGrid[p4.y, p4.x] != null)
            {
                nearIds.Add(p4);
                s += ">";
            }

        }
        
       if (pos.y>0)
       {
           SerializedVector2Int p1 = new SerializedVector2Int(pos.x, pos.y-1);
           if (Session.Value.PresetsGrid[p1.y, p1.x]!= null)
           {
               nearIds.Add(p1);
                s += "<";
            }

       }

        if (pos.x < Session.Value.PresetsGrid.GetLength(0) - 1)
        {
            SerializedVector2Int p4 = new SerializedVector2Int(pos.x+1, pos.y);
            if (Session.Value.PresetsGrid[p4.y, p4.x] != null)
            {
                nearIds.Add(p4);
                s += "A";
            }

        }

        if (pos.x > 0)
        {
            SerializedVector2Int p1 = new SerializedVector2Int(pos.x-1, pos.y);
            if (Session.Value.PresetsGrid[p1.y, p1.x] != null)
            {
                nearIds.Add(p1);
                s += "V";
            }

        }


        Debug.Log(s);

        LoadPreset(nearIds.OrderBy(id=>Guid.NewGuid()).FirstOrDefault());
    }

    private void Start()
    {
        ActivePoint.AddListener(ActivePointChanged);
        ActiveGap.AddListener(ActiveGapChanged);
        Session.AddListener(SessionChanged);
        DraggingSource.AddListener(DraggingSourceChanged);
    }

    private void DraggingSourceChanged(ISource source)
    {

        draggingSourceView.SetActive(source!=null);
        
        if (source!=null)
        {
            draggingSourceView.transform.GetChild(0).GetComponent<Image>().sprite = source.Icon;
        }
    }

    private void Update()
    {
        draggingSourceView.transform.position = Input.mousePosition;
        if (DraggingSource.Value!=null && Input.GetMouseButtonUp(0))
        {
            DraggingSource.SetState(null);
        }

        foreach (FrequencyGap fg in Session.Value.Gaps)
        {
            int start = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.Start);
            int end = Mathf.RoundToInt(SpectrumBar.SpectrumSize * fg.End);
            List<float> data = SpectrumBar.GetSpectrumData().ToList().GetRange(start, end - start);
            float dataAverage = data.Sum()/data.Count;
            fg.UpdateFrequency(data);
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
            if (preset.Points[i].Active.Value || useTemp)
            {
                Visual.SetFloat("P" + i + "Radius", preset.Points[i].Radius.Value.Value);
                Visual.SetFloat("P" + i + "Value", preset.Points[i].Volume.Value.Value);
            }
            else
            {

                Visual.SetFloat("P" + i + "Radius", 0);
                Visual.SetFloat("P" + i + "Value", 0);
            }

            if (useTemp)
            {
                Visual.SetGradient("P" + i + "Gradient".ToString(), preset.Points[i].TempGradient.Gradient);
                Visual.SetAnimationCurve("P" + i + "Func", preset.Points[i].TempCurve.Curve);
            }
            else
            {
                Visual.SetGradient("P" + i + "Gradient".ToString(), preset.Points[i].Gradient.Gradient);
                Visual.SetAnimationCurve("P" + i + "Func", preset.Points[i].Curve.Curve);
            }
        } 

    }

    public void LoadPreset(SerializedVector2Int pos)
    {

        ActivePoint.SetState(null);
        ActiveGap.SetState(null);

        if (Session.Value.ActivePreset.Value!=null)
        {
        
        }

        Session.Value.LoadPreset(pos);
        GapEditor.Init(Session.Value);

        int j = 0;
        foreach (KineticPoint kp in FindObjectsOfType<KineticPoint>().OrderBy(kp=>kp.transform.GetSiblingIndex()))
        {
            kp.Init(Session.Value.ActivePreset.Value.Points[j]);
            j++;
        }
    }

}
