using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using System.Linq;

public class BpmService : MonoBehaviour
{
    [SerializeField]
    private int BeatTreshold = 15;

    [SerializeField]
    private AudioProcessor processor;


    public ReactiveProperty<bool> Playing = new ReactiveProperty<bool>(false);

    public ReactiveProperty<int> Bpm = new ReactiveProperty<int>( 120);


    [SerializeField]
    private int Samples = 1024;

    [SerializeField]
    private int WindowSize = 200;

    [SerializeField]
    private float PeakCoef = 1.5f;


    public ReactiveCommand OnBeat { get; private set; } = new ReactiveCommand();
    public ReactiveCommand OnQuart { get; private set; } = new ReactiveCommand();


    private List<float> beats = new List<float>();

    private SpectralFluxAnalyzer realTimeSpectralFluxAnalyzer;

    private float sinceLastBeat = 0;
    private float lastClickTime;

    [Inject]
    public void Construct()
    {
       // processor.onBeat.Subscribe(_ => { Beat(); }).AddTo(this);
        realTimeSpectralFluxAnalyzer = new SpectralFluxAnalyzer(Samples, WindowSize, PeakCoef);
        //Bpm.AddListener(BpmChanged);
    }
    public void TestBeat()
    {

        if (realTimeSpectralFluxAnalyzer.spectralFluxSamples.Count >= realTimeSpectralFluxAnalyzer.thresholdWindowSize)
        {
            if (Mathf.Abs(realTimeSpectralFluxAnalyzer.Bpm - Bpm.Value) > BeatTreshold)
            {
                Bpm.Value = realTimeSpectralFluxAnalyzer.Bpm;
            }
        }
        else
        {
            if (Bpm.Value != 120)
            {
                Bpm.Value = 120;
            }
        }
    }

    public void Beat()
    {
        Debug.Log("Beat");
    }

   
    void Update()
    {
        /*
        realTimeSpectralFluxAnalyzer.analyzeSpectrum(processor.ChachedSpectrumData.Take(Samples).ToArray(), Time.timeSinceLevelLoad);

        if (realTimeSpectralFluxAnalyzer.spectralFluxSamples.Count > realTimeSpectralFluxAnalyzer.thresholdWindowSize * 1000f)
        {
            realTimeSpectralFluxAnalyzer.Reset();
        }

        TestBeat();
        */
    }


    public void ResetBpm()
    {
        realTimeSpectralFluxAnalyzer.Reset();
        TestBeat();
    }
}
