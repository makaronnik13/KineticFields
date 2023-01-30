using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using Assets.WasapiAudio.Scripts.Wasapi;
using CSCore.CoreAudioAPI;
using Assets.WasapiAudio.Scripts.Unity;
using Zenject;
using UniRx;

namespace KineticFields
{
    public class FFTService: MonoBehaviour
    {
        [SerializeField] private bool autoGain;
        
        public List<SourceVariant> SourceVariants { get; private set; } = new List<SourceVariant>();
        public ReactiveProperty<SourceVariant> CaptureDevice { get; private set; } = new ReactiveProperty<SourceVariant>(null);

        public ReactiveCommand OnDeviceListChanged { get; private set; } = new ReactiveCommand();

        public int DeviceId => SourceVariants.IndexOf(CaptureDevice.Value);

        [SerializeField] private List<WasapiAudioSource> Sources = new List<WasapiAudioSource>();
        [SerializeField]
        private AudioVisualizationProfile profile;

        private Dictionary<WasapiAudioSource, float[]> cachedSpectrums = new Dictionary<WasapiAudioSource, float[]>();
        private float multiplyer = 1;
        private float autoGainRelaxationTime = 5f;
        private MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();


        [Inject]
        public void Construct()
        {
            CaptureDevice.Subscribe(device =>
            {
                if (device==null)
                {
                    return;
                }
                foreach (WasapiAudioSource source in Sources)
                {
                    source.SetSourceType(device);
                }
            }).AddTo(this);

            //add loopback variant
            SourceVariants.Add(new SourceVariant());


            deviceEnumerator = new MMDeviceEnumerator();
            
        }

        private void Update()
        {
            ManageDevices();
            ManageSignalMultiplyer();
            CacheSpectrums();  
        }

        private void CacheSpectrums()
        {
            foreach (WasapiAudioSource source in Sources)
            {
                if (!cachedSpectrums.ContainsKey(source))
                {
                    cachedSpectrums.Add(source, source.GetSpectrumData(AudioVisualizationStrategy.Scaled));
                }
                else
                {
                    cachedSpectrums[source] = source.GetSpectrumData(AudioVisualizationStrategy.Scaled);
                }
            }
        }


        private float[] ApplyFrofile(float[] data)
        {
            bool smoothed = profile.AudioSmoothingIterations != 0;
        //    var scaledSpectrumData = new float[data.Length];
        //    var scaledMinMaxSpectrumData = new float[SpectrumSize];

            // Apply AudioVisualizationProfile
            var scaledMax = 0.0f;
            var scaledAverage = 0.0f;
            var scaledTotal = 0.0f;
            var scaleStep = 1.0f / data.Length;

            /*
            // 2: Scaled. Scales against animation curve
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = profile.ScaleCurve.Evaluate(scaleStep * i) * data[i];

                if (data[i] > scaledMax)
                {
                    scaledMax = data[i];
                }

                scaledTotal += scaledValue;
            }
            */


            // 3: MinMax
            scaledAverage = scaledTotal / data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                var scaledValue = data[i];
                var cutoff = scaledAverage * profile.MinMaxThreshold;

                if (scaledValue <= cutoff)
                {
                    scaledValue *= profile.MinScale;
                }
                else if (scaledValue >= cutoff)
                {
                    scaledValue *= profile.MaxScale;
                }

                data[i] = scaledValue;
            }

            /*
            // 4: Smoothed

            // We need a smoother for each combination of SpectrumSize/Iteration/Strategy
            var smootherId = $"{data.Length}-{profile.AudioSmoothingIterations}-{ScalingStrategy.Sqrt}";
            if (!_spectrumSmoothers.ContainsKey(smootherId))
            {
                _spectrumSmoothers.Add(smootherId, new SpectrumSmoother(SpectrumSize, profile.AudioSmoothingIterations));
            }

            var smoother = _spectrumSmoothers[smootherId];
            */


            /*
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
            */


             data = data.Select(v => v * multiplyer).ToArray();
      
            /*
            if (smoothed)
            {
                result = smoother.GetSpectrumData(result);
            }*/

            return data;
            
        }


        private void ManageSignalMultiplyer()
        {
            if (autoGain)
            {
                if (cachedSpectrums.Count == 0)
                {
                    return;
                }

                // GetSpectrumData(AudioVisualizationStrategy.Scaled, Sources[0].Profile, false);
                if(cachedSpectrums[Sources[0]].Count() == 0)
                {
                    return;
                }
               
                float maxMult = cachedSpectrums[Sources[0]].Max() * Sources[0].Multiplyer;

                // float max = Sources[0].GetSpectrumData(AudioVisualizationStrategy.Scaled, Sources[0].Profile, true).Max();
                //Debug.Log(Sources[0].GetSpectrumData(AudioVisualizationStrategy.Scaled, true, Sources[0].Profile, false).Max());
                //Debug.Log(maxMult);

                if (maxMult>1f)
                {
                    multiplyer -= Time.deltaTime / autoGainRelaxationTime;
                }
                else
                {
                    multiplyer += Time.deltaTime / autoGainRelaxationTime;
                }
            }
            else
            {
                multiplyer = 1;
            }
            
            foreach (WasapiAudioSource source in Sources)
            {
                source.Multiplyer = multiplyer;
            }
        }

        private void ManageDevices()
        {
            bool changed = false;
                
          
                MMDeviceCollection newDevices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                    
                if (SourceVariants.Count!=newDevices.Count+1)
                {
                    for (int i = SourceVariants.Count - 1; i >= 0; i--)
                    {
                        if (SourceVariants[i].CaptureType == WasapiCaptureType.Microphone && !newDevices.Contains(SourceVariants[i].Device))
                        {
                            SourceVariants.Remove(SourceVariants[i]);
                            changed = true;
                        }
                    }

                    foreach (MMDevice device in newDevices)
                    {
                        if (SourceVariants.FirstOrDefault(v=>v.Device == device)==null)
                        {
                            SourceVariants.Add(new SourceVariant(device));
                            changed = true;
                        }   
                    }
                        
                    if (!SourceVariants.Contains(CaptureDevice.Value))
                    {
                        CaptureDevice.Value = SourceVariants.FirstOrDefault(s => s.CaptureType == WasapiCaptureType.Loopback);
                    }
                }

        

            if (changed)
            {
                OnDeviceListChanged.Execute();
            }
        }

        public void SelectDevice(int i)
        {
            CaptureDevice.Value = SourceVariants[i];
        }

        public float[] GetRawSpectrum(int i)
        {
            return Sources[i].GetSpectrumData(AudioVisualizationStrategy.Raw);
        }
        
        public float[] GetSpectrumGap(FrequencyGap gap)
        {
            WasapiAudioSource source = Sources[0];
            int minFrequency = 0;
            int maxFrequency = 0;

            switch (gap)
            {
                case FrequencyGap.None:
                    source = Sources[0];
                    minFrequency = 0;
                    maxFrequency = source.SpectrumSize;
                    break;
                case FrequencyGap.SubBass:
                    source = Sources[1];
                    minFrequency = 0;
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.1f);
                    break;
                case FrequencyGap.Bass:
                    source = Sources[1];
                    minFrequency =  Mathf.RoundToInt(source.SpectrumSize*0.1f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.25f);
                    break;
                case FrequencyGap.LowMidrange:
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.2f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.4f);
                    source = Sources[2];
                    break;
                case FrequencyGap.Midrange:
                    source = Sources[2];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.4f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.6f);
                    break;
                case FrequencyGap.UpperMidrange:
                    source = Sources[2];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.6f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.8f);
                    break;
                case FrequencyGap.Presence:
                    source = Sources[3];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.7f);
                    maxFrequency = source.SpectrumSize;
                    break;
            }
            
            try
            {
                return cachedSpectrums[source].Select(s => s * source.Multiplyer).ToList().GetRange(minFrequency, maxFrequency - minFrequency).ToArray();
            }
            catch
            {
                return cachedSpectrums[source];
            }
        }
  
        public float[] GetSpectrum(int spectrumId)
        {
            return cachedSpectrums[Sources[spectrumId]];
        }

        public int GetSpectrumGapSize(FrequencyGap gap)
        {
            int minFrequency = 0;
            int maxFrequency = 0;

            WasapiAudioSource source;
            
            switch (gap)
            {
                case FrequencyGap.None:
                    source = Sources[0];
                    minFrequency = 0;
                    maxFrequency = source.SpectrumSize;
                    break;
                case FrequencyGap.SubBass:
                    source = Sources[1];
                    minFrequency = 0;
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.1f);
                    break;
                case FrequencyGap.Bass:
                    source = Sources[1];
                    minFrequency =  Mathf.RoundToInt(source.SpectrumSize*0.1f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.25f);
                    break;
                case FrequencyGap.LowMidrange:
                    source = Sources[2];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.2f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.4f);
                    break;
                case FrequencyGap.Midrange:
                    source = Sources[2];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.4f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.6f);
                    break;
                case FrequencyGap.UpperMidrange:
                    source = Sources[2];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.6f);
                    maxFrequency = Mathf.RoundToInt(source.SpectrumSize*0.8f);
                    break;
                case FrequencyGap.Presence:
                    source = Sources[3];
                    minFrequency = Mathf.RoundToInt(source.SpectrumSize*0.7f);
                    maxFrequency = source.SpectrumSize;
                    break;
                default:
                    source = Sources[0];
                    break;
            }
            return maxFrequency - minFrequency;
        }
    }
}
