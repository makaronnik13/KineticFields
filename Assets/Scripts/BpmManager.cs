using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KineticFields;
using UniRx;
using UnityEngine;
using Zenject;

public class BpmManager : MonoBehaviour
{
    [SerializeField]
    private int WindowSize = 5;

    [SerializeField]
    private float PeakCoef = 1.5f;

    [SerializeField]
    private float repeatRate = 0.03f;
    
    public ReactiveProperty<int> Bpm { get; private set; } = new ReactiveProperty<int>(120);

    public ReactiveCommand OnBeat { get; private set; } = new ReactiveCommand();

    private Dictionary<int, SpectralFluxAnalyzer> realTimeSpectralFluxAnalyzers = new Dictionary<int, SpectralFluxAnalyzer>();

    private float sinceLastBeat = 0;
    private float lastClickTime;
 
    [Inject]
    public void Construct(FFTService fftService)
    {
        int realWindowSize = WindowSize;
        realTimeSpectralFluxAnalyzers.Add(0, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.None), realWindowSize, PeakCoef));
        realTimeSpectralFluxAnalyzers.Add(1, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.None), realWindowSize, PeakCoef));
        //realTimeSpectralFluxAnalyzers.Add(FrequencyGap.SubBass, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.SubBass), realWindowSize, PeakCoef));
        //realTimeSpectralFluxAnalyzers.Add(FrequencyGap.LowMidrange, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.LowMidrange), realWindowSize, PeakCoef));
        //realTimeSpectralFluxAnalyzers.Add(FrequencyGap.Midrange, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.Midrange), realWindowSize, PeakCoef));
        realTimeSpectralFluxAnalyzers.Add(2, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.None), realWindowSize, PeakCoef));
        realTimeSpectralFluxAnalyzers.Add(3, new SpectralFluxAnalyzer(fftService.GetSpectrumGapSize(FrequencyGap.None), realWindowSize, PeakCoef));
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
            string bpms = "";
            float sum = 0;
            
            foreach (KeyValuePair<int, SpectralFluxAnalyzer> pair in realTimeSpectralFluxAnalyzers)
            {
                if (pair.Value.spectralFluxSamples.Count>Mathf.RoundToInt(WindowSize / repeatRate))
                {
                    Debug.Log( $"{pair.Key}: {pair.Value.Bpm}");
                    pair.Value.Reset();
                }
                
                pair.Value.thresholdMultiplier = PeakCoef;
                pair.Value.thresholdWindowSize = WindowSize;
                pair.Value.analyzeSpectrum(fftService.GetRawSpectrum(pair.Key).ToArray(), Time.timeSinceLevelLoad);
                bpms += $"{pair.Key}: {pair.Value.Bpm}\n";
                sum += pair.Value.Bpm;
            }
            
            //Debug.Log(sum+"/"+realTimeSpectralFluxAnalyzers.Count);
            float average = (float)sum / realTimeSpectralFluxAnalyzers.Count;
            //Debug.Log("avg: "+average);
            //Debug.Log(bpms);
        }).AddTo(this);
    }
}
