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
        //public AudioVisualizationProfile Profile;


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

        public float[] GetSpectrumData(AudioVisualizationStrategy strategy)
        {   

            if (_spectrumData == null)
            {
                return new float[0];
            }

            return _spectrumData;

            
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
