﻿using Assets.WasapiAudio.Scripts.Unity;
using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BpmManager : AudioVisualizationEffect
{
    [SerializeField]
    private TMPro.TextMeshProUGUI BpmLable;

    public static BpmManager Instance;

    [SerializeField]
    private AudioProcessor processor;

    private Coroutine swapCoroutine;

    public GenericFlag<bool> Playing = new GenericFlag<bool>("isPlaying", false);

    public GenericFlag<int> Bpm = new GenericFlag<int>("bpm", 120);


    [SerializeField]
    private int Samples = 1024;

    [SerializeField]
    private int WindowSize = 200;

    [SerializeField]
    private float PeakCoef = 1.5f;

    [SerializeField]
    private GameObject Marker, Circles;

    private Coroutine detectionCoroutine, spawnMarkerCoroutine, tapDetectionCoroutine;


    public Action OnBeat = () => { };

    private List<float> beats = new List<float>();

    private SpectralFluxAnalyzer realTimeSpectralFluxAnalyzer;

    private float sinceLastBeat = 0;
    private float lastClickTime;
    private float timer;

    //private Coroutine oscilatorsCoroutine;

    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        //processor.onBeat.AddListener(Beat);
        Bpm.AddListener(BpmChanged);

        realTimeSpectralFluxAnalyzer = new SpectralFluxAnalyzer(Samples, WindowSize, PeakCoef);

        spawnMarkerCoroutine = StartCoroutine(SpawnMarker());

       
    }

    private IEnumerator SpawnMarker()
    {
        while (true)
        {
            if (Bpm.Value>0)
            {
                CreateMarker();
            }

            yield return new WaitForSeconds(60f/Bpm.Value);
        }
    }

    private void CreateMarker()
    {
        GameObject newMarker = Instantiate(Marker);
        newMarker.transform.SetParent(transform.GetChild(0));
        newMarker.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    private void BpmChanged(int v)
    {
        BpmLable.text = v + "bpm";
        foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
        {
            osc.Reset();
        }
        //Debug.Log(v);
    }



    public void Beat()
    {
        GameObject circles = Instantiate(Circles);
        circles.transform.SetParent(transform.GetChild(0));
        circles.transform.localPosition = Vector3.zero;
        Destroy(circles, 1);

        //beats.Add(Time.timeSinceLevelLoad);

        //beats.RemoveAll(b=>Time.timeSinceLevelLoad-b>DetectionTimeGap);

        //bpm.SetState(beats.Count/Mathf.Min(DetectionTimeGap, Time.timeSinceLevelLoad) *60f);

        OnBeat.Invoke();

        if (Playing.Value)
        {
            KineticFieldController.Instance.RandomSwap();
        }

        /*if (oscilatorsCoroutine!=null)
        {
            StopCoroutine(oscilatorsCoroutine);
        }*/

        //oscilatorsCoroutine = StartCoroutine(UpdateOscilators());

        foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
        {
            osc.Beat();
        }
    }

    /*
    private IEnumerator UpdateOscilators()
    {
        float gap = 60f / Bpm.Value;
        float time = gap;
        while (time >= 0)
        {
            foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
            {
                osc.UpdateOscilator(1f-time/gap, beatId);
            }
            time -= Time.deltaTime;
            yield return null;
        }
    }
    */

    void Update()
    {
        if (Playing.Value)
        {
            timer += Time.deltaTime;
        }

        if (KineticFieldController.Instance.KeysEnabled && EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (Playing.Value)
                {
                    Debug.Log("Stop");
                    //stop
                    Playing.SetState(false);
                }
                else
                {
                    //play
                    Debug.Log("Play");
                    Playing.SetState(true);
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                if (tapDetectionCoroutine != null)
                {
                    StopCoroutine(tapDetectionCoroutine);
                }
                tapDetectionCoroutine = StartCoroutine(TapDetection());
                beats.Add(Time.timeSinceLevelLoad);

                if (beats.Count > 1)
                {
                    float newBpm = 0;

                    newBpm = beats[beats.Count - 1] - beats[0];

                    newBpm /= beats.Count - 1f;

                    Bpm.SetState(Mathf.RoundToInt(60f / newBpm));

                    if (spawnMarkerCoroutine != null)
                    {
                        StopCoroutine(spawnMarkerCoroutine);
                    }
                    spawnMarkerCoroutine = StartCoroutine(SpawnMarker());
                }
                // StartDetection();


            }

            if (Input.GetMouseButtonUp(2))
            {
                // StopDetection();
            }
        }

        foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
        {
            osc.UpdateOscilator();
        }
    }

    private IEnumerator TapDetection()
    {
        yield return new WaitForSeconds(2f);
        beats.Clear();
    }

    public void StartDetection()
    {
        if (detectionCoroutine!=null)
        {
            StopCoroutine(detectionCoroutine);
        }

       
        detectionCoroutine = StartCoroutine(DetectionCoroutine());
    }

    public void StopDetection()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
        }

        realTimeSpectralFluxAnalyzer = new SpectralFluxAnalyzer(Samples, WindowSize, PeakCoef);

        if (spawnMarkerCoroutine != null)
        {
            StopCoroutine(spawnMarkerCoroutine);
        }

        spawnMarkerCoroutine = StartCoroutine(SpawnMarker());

        Debug.Log("!"+Bpm.Value+"!");
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            realTimeSpectralFluxAnalyzer.analyzeSpectrum(GetSpectrumData().Take(Samples).ToArray(), Time.timeSinceLevelLoad);


            //bpm.SetState(Mathf.Lerp(bpm.Value, realTimeSpectralFluxAnalyzer.Bpm, 0.2f));

            if (realTimeSpectralFluxAnalyzer.spectralFluxSamples.Count>=WindowSize)
            {
                Bpm.SetState(realTimeSpectralFluxAnalyzer.Bpm);
            }

            yield return null;
        }
    }
}