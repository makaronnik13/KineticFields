using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using CSCore.CoreAudioAPI;
using Assets.WasapiAudio.Scripts.Unity;
using Zenject;
using UniRx;
using System.Collections;
using ModestTree;

namespace KineticFields
{
    public class FFTService: MonoBehaviour
    {
        [SerializeField] private bool autoGain;
        [SerializeField] WindowFunctionType windowFunctionType = WindowFunctionType.BlackmannHarris;
        [SerializeField] ScalingStrategy scalingStrategy = ScalingStrategy.Sqrt;
        [SerializeField] int spectumSize = 4096;
        public List<SourceVariant> SourceVariants { get; private set; } = new List<SourceVariant>();
        public ReactiveProperty<SourceVariant> CaptureDevice { get; private set; } = new ReactiveProperty<SourceVariant>(null);

        public ReactiveCommand OnDeviceListChanged { get; private set; } = new ReactiveCommand();

        public int DeviceId => SourceVariants.IndexOf(CaptureDevice.Value);

        [SerializeField] private List<WasapiAudioSource> Sources = new List<WasapiAudioSource>();
        private List<SpectrumReceiver> Recievers = new List<SpectrumReceiver>();

       // [SerializeField] private AudioVisualizationProfile profile;

       // private Dictionary<SpectrumReceiver, float[]> cachedSpectrums = new Dictionary<SpectrumReceiver, float[]>();

        [SerializeField]
        public float multiplyer = 1;

        [SerializeField]
        private float autoGainRelaxationTime = 5f;
        private MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();

        private Dictionary<FrequencyGap, float[]> gapsValues = new Dictionary<FrequencyGap, float[]>();
        private readonly Dictionary<FrequencyGap, float[]> scaledGapsValues = new Dictionary<FrequencyGap, float[]>();

        private bool deviceHasChanged = false;

        private MMDeviceCollection devices;
        private WasapiAudioSource source;
        private SpectrumReceiver reciever;


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
               
                    //source.SetSourceType(device);
                }
            }).AddTo(this);


            foreach (FrequencyGap gap in Enum.GetValues(typeof(FrequencyGap)))
            {
                switch (gap)
                {
                    case FrequencyGap.None:
                        source = Sources[0];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, 0, spectumSize, (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.SubBass:
                        source = Sources[1];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, 0, Mathf.RoundToInt(spectumSize * 0.015f), (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.Bass:
                        source = Sources[1];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, Mathf.RoundToInt(spectumSize * 0.015f), Mathf.RoundToInt(spectumSize * 0.1f), (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.LowMidrange:
                        source = Sources[2];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, Mathf.RoundToInt(spectumSize * 0.1f), Mathf.RoundToInt(spectumSize * 0.4f), (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.Midrange:
                        source = Sources[2];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, Mathf.RoundToInt(spectumSize * 0.4f), Mathf.RoundToInt(spectumSize * 0.7f), (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.UpperMidrange:
                        source = Sources[3];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, Mathf.RoundToInt(spectumSize * 0.7f), Mathf.RoundToInt(spectumSize * 0.8f), (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                    case FrequencyGap.Presence:
                        source = Sources[3];
                        reciever = new SpectrumReceiver(spectumSize, scalingStrategy, windowFunctionType, Mathf.RoundToInt(spectumSize * 0.8f), spectumSize, (spectrum) =>
                        {
                            ProcessGap(gap, spectrum);
                        });
                        break;
                }
                
                source.AddReceiver(reciever);
                gapsValues.Add(gap, new float[0]);
                scaledGapsValues.Add(gap, new float[0]);
            }
            
            
        }

        private void ProcessGap(FrequencyGap gap, float[] spectrum)
        {
            
            gapsValues[gap] = spectrum;
            if (scaledGapsValues[gap].Length != gapsValues[gap].Length)
            {
                scaledGapsValues[gap] = gapsValues[gap];
            }
            for (int i = 0; i < scaledGapsValues[gap].Length; i++)
            {
                scaledGapsValues[gap][i] = gapsValues[gap][i] * multiplyer;
            }
        }

        private void Start()
        {
            ManageDevices();
        }

        private void Update()
        {
            ManageDevices();
            //ManageSignalMultiplyer();
        }
        
 


        private void ManageSignalMultiplyer()
        {
            if (autoGain)
            {
                if (multiplyer > 1f)
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
            
        }

        private void ManageDevices()
        {
            SourceVariants.Add(new SourceVariant());

            deviceEnumerator = new MMDeviceEnumerator();

            devices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                    
            if (SourceVariants.Count!=devices.Count+1)
                {
                    for (int i = SourceVariants.Count - 1; i >= 0; i--)
                    {
                        if (SourceVariants[i].CaptureType == WasapiCaptureType.Microphone && !devices.Contains(SourceVariants[i].Device))
                        {
                            SourceVariants.Remove(SourceVariants[i]);
                            deviceHasChanged = true;
                        }
                    }

                    foreach (MMDevice device in devices)
                    {
                        if (SourceVariants.FirstOrDefault(v=>v.Device == device)==null)
                        {
                            SourceVariants.Add(new SourceVariant(device));
                            deviceHasChanged = true;
                        }   
                    }
                        
                    if (!SourceVariants.Contains(CaptureDevice.Value))
                    {
                        CaptureDevice.Value = SourceVariants.FirstOrDefault(s => s.CaptureType == WasapiCaptureType.Loopback);
                    }
                }

        

            if (deviceHasChanged)
            {
                OnDeviceListChanged.Execute();
                deviceHasChanged = false;
            }
        }

        public void SelectDevice(int i)
        {
            CaptureDevice.Value = SourceVariants[i];
        }
        
        public float[] GetSpectrumGap(FrequencyGap gap)
        {
            return scaledGapsValues[gap];
        }

    }
}
