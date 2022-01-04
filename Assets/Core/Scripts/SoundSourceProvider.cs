using Assets.WasapiAudio.Scripts.Unity;
using CSCore.CoreAudioAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;


public class SoundSourceProvider : MonoBehaviour, ITickable
{
    private SourceService sourceService;
    private List<FrequencyGap> gaps = new List<FrequencyGap>();
    private MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
    private MMDeviceCollection devices = null;

   public ReactiveProperty<MMDevice> ActiveSoundDevice = new ReactiveProperty<MMDevice>(null);



    [SerializeField]
    private WasapiAudioSource AudioSource;
   // [SerializeField]
   // private AudioVisualizationProfile AudioProfile, MicProfile;

    public ReactiveCollection<MMDevice> Devices;

    [Inject]
    void Construct(SourceService sourceService)
    {
        this.sourceService = sourceService;

        //create 3 gaps
        AddGap("Fire", 0.1f, 0.3f, Color.red, DefaultResources.GapSprites[1]);  //replace default resources with resourceService
        AddGap("Air", 0.3f, 0.3f, Color.cyan, DefaultResources.GapSprites[2]);
        AddGap("Earth", 0.6f, 0.3f, Color.green, DefaultResources.GapSprites[3]);
        AddGap("Water", 0.9f, 0.3f, Color.blue, DefaultResources.GapSprites[4]);

        //AudioSource.CaptureType

        ActiveSoundDevice.Subscribe((v) =>
        {
            AudioSource.SetSourceType(v);
        });
    }



    public FrequencyGap AddGap(string name, float pos, float size, Color color, Sprite sprite)
    {
        FrequencyGap fg = new FrequencyGap(name, pos, size, color, sprite);
        gaps.Add(fg);
        sourceService.RegisterSource(fg);
        return fg;
    }

    public void SelectSoundSource(int v)
    {
        if (v == 0)
        {
            ActiveSoundDevice.Value = null;
        }
        else
        {
            ActiveSoundDevice.Value = devices.ItemAt(v - 1);
        }
    }

    public void Tick()
    {
            MMDeviceCollection newDevices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
            if (devices == null || newDevices.Count != devices.Count)
            {
                devices = newDevices;
                Devices.Clear();
                foreach (MMDevice device in newDevices)
                {
                    Devices.Add(device);
                }
            }
    }

    public MMDevice GetDefaultDevice()
    {
        return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
    }
}
