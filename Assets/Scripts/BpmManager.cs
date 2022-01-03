using Assets.WasapiAudio.Scripts.Unity;
using com.armatur.common.flags;
using Resolink;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BpmManager : MonoBehaviour
{
    [SerializeField]
    private int BeatTreshold = 15;

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
    public Action OnQuart = () => { };

    private List<float> beats = new List<float>();

    private SpectralFluxAnalyzer realTimeSpectralFluxAnalyzer;

    private float sinceLastBeat = 0;
    private float lastClickTime;

    private AudioDecorator audio;

    [Inject]
    public void Construct(AudioDecorator audio)
    {
        this.audio = audio;
    }

    public void Awake()
    {
        Instance = this;
        FindObjectOfType<SettingsPanel>().OnResolinkStateChanged += ResolinkStateChanged;
    }

    private void ResolinkStateChanged(bool v)
    {
        if (v)
        {
            FindObjectOfType<TimeManager>().BpmChanged += BpmChangedFromResolink;
        }
        else
        {
            FindObjectOfType<TimeManager>().BpmChanged -= BpmChangedFromResolink;
        }
        
    }

    private void BpmChangedFromResolink(int v)
    {
        Bpm.SetState(v);
        KineticFieldController.Instance.SyncOsc();
        TracksManager.Instance.RandomSwap();
    }

    // Start is called before the first frame update
    void Start()
    {
        //processor.onBeat.AddListener(TestBeat);
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

            for (int i = 0; i < 4; i++)
            {
                float t = 0;
                OnQuart();
                while (t< 15f / Bpm.Value)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            OnBeat();

        }
    }

    private void CreateMarker()
    {

        GameObject newMarker = Marker.Spawn(transform.GetChild(0), Vector3.zero);
        newMarker.name = transform.GetChild(0).childCount + "_marker";
        newMarker.transform.SetParent(transform.GetChild(0));
        newMarker.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newMarker.GetComponent<BpmMarker>().Reset();
        StartCoroutine(DelayDestroy(newMarker, 200f/Bpm.Value));
    }

    private void BpmChanged(int v)
    {
        BpmLable.text = v + "bpm";

        if (KineticFieldController.Instance.Session.Value!=null)
        {
            foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
            {
               // osc.Reset();
            }
        }
        
    }

    public void TestBeat()
    {
        if (SettingsPanel.Instance.Resolink.activeInHierarchy)
        {
            return;
        }
        if (realTimeSpectralFluxAnalyzer.spectralFluxSamples.Count >= realTimeSpectralFluxAnalyzer.thresholdWindowSize)
        {
            if (Mathf.Abs(realTimeSpectralFluxAnalyzer.Bpm - Bpm.Value) > BeatTreshold)
            {
                Bpm.SetState(realTimeSpectralFluxAnalyzer.Bpm);

                /*
                if (spawnMarkerCoroutine != null)
                {
                    StopCoroutine(spawnMarkerCoroutine);
                }
                spawnMarkerCoroutine = StartCoroutine(SpawnMarker());
                */
            }
        }
        else
        {
            if (Bpm.Value!=120)
            {
                Bpm.SetState(120);

                /*
                if (spawnMarkerCoroutine != null)
                {
                    StopCoroutine(spawnMarkerCoroutine);
                }
                spawnMarkerCoroutine = StartCoroutine(SpawnMarker());
                */


            }
        }
    }

    public void Beat()
    {
        GameObject circles = Circles.Spawn();
        circles.transform.SetParent(transform.GetChild(0));
        circles.transform.localPosition = Vector3.zero;

    
        StartCoroutine(DelayDestroy(circles, 1f));

        OnBeat.Invoke();


        if (KineticFieldController.Instance.Session.Value!=null)
        {
            foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
            {
                osc.Beat();
            }
        }
    }

    private IEnumerator DelayDestroy(GameObject obj, float v)
    {
        yield return new WaitForSeconds(v);

        obj.Recycle();
    }



    void Update()
    {
        realTimeSpectralFluxAnalyzer.analyzeSpectrum(audio.Spectrum.Take(Samples).ToArray(), Time.timeSinceLevelLoad);

        if (realTimeSpectralFluxAnalyzer.spectralFluxSamples.Count>realTimeSpectralFluxAnalyzer.thresholdWindowSize*1000f)
        {
            realTimeSpectralFluxAnalyzer.Reset();
        }

        if (KineticFieldController.Instance.Session.Value!=null)
        {
            foreach (Oscilator osc in KineticFieldController.Instance.Session.Value.Oscilators)
            {
                osc.UpdateOscilator();
            }
        }

        TestBeat();
    }


    public void ResetBpm()
    {
        realTimeSpectralFluxAnalyzer.Reset();
        TestBeat();
    }
}
