using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using Assets.WasapiAudio.Scripts.Wasapi;
using CSCore.CoreAudioAPI;
using UnityEngine;
using Zenject;
using UniRx;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class WasapiAudioSource : MonoBehaviour
    {
        private readonly Dictionary<string, SpectrumSmoother> _spectrumSmoothers = new Dictionary<string, SpectrumSmoother>();

        private Wasapi.WasapiAudio _wasapiAudio;
        private float[] _spectrumData;

        // Inspector Properties
        public ReactiveProperty<WasapiCaptureType> CaptureType = new ReactiveProperty<WasapiCaptureType>(WasapiCaptureType.Loopback);

        
        public int SpectrumSize = 512;

        public float Multiplyer = 1f;
        public ScalingStrategy ScalingStrategy = ScalingStrategy.Sqrt;
        public int MinFrequency = 20;
        public int MaxFrequency = 10000;
        public WasapiAudioFilter[] Filters;
        public AudioVisualizationProfile Profile;


        [Inject]
        public void Construct()
        {
        }



        
        public void SetSourceType(SourceVariant sourceVariant)
        {
            _wasapiAudio?.StopListen();
            WasapiCaptureType wct = sourceVariant.CaptureType;

            CaptureType.Value = wct;

            _wasapiAudio = new Wasapi.WasapiAudio(wct, SpectrumSize, ScalingStrategy, MinFrequency, MaxFrequency, Filters, spectrumData =>
            {
                _spectrumData = spectrumData;
            });

            _wasapiAudio.StartListen(sourceVariant);
        }

        public void Update()
        {
            foreach (var smoother in _spectrumSmoothers.Values)
            {
                smoother.AdvanceFrame();
            }
        }

        public float[] GetSpectrumData(AudioVisualizationStrategy strategy,  AudioVisualizationProfile profile, bool useMultiplyer = true)
        {
            bool smoothed = profile.AudioSmoothingIterations != 0;
            
            if (_spectrumData == null)
            {
                return new float[0];
            }

            var scaledSpectrumData = new float[SpectrumSize];
            var scaledMinMaxSpectrumData = new float[SpectrumSize];

            // Apply AudioVisualizationProfile
            var scaledMax = 0.0f;
            var scaledAverage = 0.0f;
            var scaledTotal = 0.0f;
            var scaleStep = 1.0f / SpectrumSize;

            // 2: Scaled. Scales against animation curve
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = profile.ScaleCurve.Evaluate(scaleStep * i) * _spectrumData[i];
                scaledSpectrumData[i] = scaledValue;

                if (scaledSpectrumData[i] > scaledMax)
                {
                    scaledMax = scaledSpectrumData[i];
                }

                scaledTotal += scaledValue;
            }

            // 3: MinMax
            scaledAverage = scaledTotal / SpectrumSize;
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = scaledSpectrumData[i];
                var cutoff = scaledAverage * profile.MinMaxThreshold;

                if (scaledValue <= cutoff)
                {
                    scaledValue *= profile.MinScale;
                }
                else if (scaledValue >= cutoff)
                {
                    scaledValue *= profile.MaxScale;
                }

                scaledMinMaxSpectrumData[i] = scaledValue;
            }

            // 4: Smoothed

            // We need a smoother for each combination of SpectrumSize/Iteration/Strategy
            var smootherId = $"{SpectrumSize}-{profile.AudioSmoothingIterations}-{strategy}";
            if (!_spectrumSmoothers.ContainsKey(smootherId))
            {
                _spectrumSmoothers.Add(smootherId, new SpectrumSmoother(SpectrumSize, profile.AudioSmoothingIterations));
            }

            var smoother = _spectrumSmoothers[smootherId];

            float[] result = null;
            
            switch (strategy)
            {
                case AudioVisualizationStrategy.Raw:
                    result = _spectrumData;
                    break;
                case AudioVisualizationStrategy.Scaled:
                    result = scaledSpectrumData;
                    break;
                case AudioVisualizationStrategy.ScaledMinMax:
                    result = scaledMinMaxSpectrumData;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid strategy: {strategy}");
            }
            
            if (useMultiplyer)
            {
                result = result.Select(v => v * Multiplyer).ToArray();
            }

            if (smoothed)
            {
                result = smoother.GetSpectrumData(result);
            }

            return result;
        }

        public void OnApplicationQuit()
        {
            if (_wasapiAudio != null)
            {
                _wasapiAudio.StopListen();
            }
        }
    }
}
