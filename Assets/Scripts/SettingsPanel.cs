using Assets.Scripts;
using Assets.WasapiAudio.Scripts.Unity;
using com.armatur.common.flags;
using CSCore.CoreAudioAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Windows.Kinect;

public class SettingsPanel : Singleton<SettingsPanel>
{
    [SerializeField]
    private GameObject ScreenTogglePrefab;

    [SerializeField]
    private Transform ScreensHub;

    [SerializeField]
    private GameObject Resolink;

    [SerializeField]
    private Toggle ResolinkToggle;

    [SerializeField]
    private BarSpectrum Bar;

    [SerializeField]
    private AudioProcessor Processor;

    [SerializeField]
    private AudioVisualizationProfile AudioProfile, MicProfile;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    WasapiAudioSource source;

    [SerializeField]
    TMPro.TMP_Dropdown  VisualDropdown, ModelDropdown, SoundSourceDropdown;

    [SerializeField]
    private VisualEffect Effect;

    public GenericFlag<KineticModel> ActiveModel = new GenericFlag<KineticModel>("ActiveModel", null);

    private MMDeviceCollection devices = null;
    private MMDevice soundSourceDdevice = null;

    private List<ScreenToggle> screenToggles = new List<ScreenToggle>();

    // Start is called before the first frame update
    void Start()
    {
        VisualDropdown.onValueChanged.AddListener(VisualChanged);
        SoundSourceDropdown.onValueChanged.AddListener(SoundSourceChanged);

        KinectSensor sensor = FindObjectOfType<KinectPointCloudMapped>().Sensor;


        if (sensor == null)
        {
            Debug.Log("Mesh");
            VisualChanged(1);
        }
        else
        {
            VisualChanged(0);
        }

        ModelDropdown.options.Clear();

        foreach (KineticModel km in DefaultResources.Models)
        {
            ModelDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(km.name));
        }

        ModelDropdown.onValueChanged.AddListener(ModelDropdownChanged);

        ActiveModel.SetState(DefaultResources.Models[0]);
        ActiveModel.AddListener(ModelChanged);
        ModelDropdown.value = 0;


        StartCoroutine(ProfileSmooth());

        ResolinkToggle.onValueChanged.AddListener(ToggleResolink);
    }

    private void ToggleResolink(bool v)
    {
        Resolink.gameObject.SetActive(v);
    }

    private IEnumerator ProfileSmooth()
    {
        MicProfile.AudioSmoothingIterations = 1;
        AudioProfile.AudioSmoothingIterations = 1;
        yield return null;
        MicProfile.AudioSmoothingIterations = 2;
        AudioProfile.AudioSmoothingIterations = 2;
    }

    private void SoundSourceChanged(int v)
    {
        if (v == 0)
        {
            soundSourceDdevice = null;
            Bar.Profile = AudioProfile;
        }
        else
        {
            soundSourceDdevice = devices.ItemAt(v-1);
            Bar.Profile = MicProfile;
        }
        source.SetSourceType(soundSourceDdevice);
    }

    private void Update()
    {
        
        using (var deviceEnumerator = new MMDeviceEnumerator())
        {
            MMDeviceCollection newDevices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
            if (devices == null || newDevices.Count!=devices.Count)
            {
                devices = newDevices;

                List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
                options.Add(new TMPro.TMP_Dropdown.OptionData("System sound"));
                foreach (MMDevice device in devices)
                {
                    options.Add(new TMPro.TMP_Dropdown.OptionData(device.FriendlyName.Split('(', ')')[1]));
                }

                SoundSourceDropdown.ClearOptions();
                SoundSourceDropdown.AddOptions(options);
      
                if (soundSourceDdevice == null || options.FirstOrDefault(o=>o.text == soundSourceDdevice.FriendlyName.Split('(', ')')[1])==null)
                {
                    if (options.Count == 0)
                    {
                        SoundSourceDropdown.value = 0;
                    }
                    else
                    {
                        SoundSourceDropdown.value = devices.ToList().IndexOf(deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia))+1;
                    }
                }
                SoundSourceDropdown.RefreshShownValue();
                SoundSourceChanged(SoundSourceDropdown.value);
            }

        }
     
    }

    private void ModelDropdownChanged(int v)
    {
        ActiveModel.SetState(DefaultResources.Models[v]);
    }

    private void ModelChanged(KineticModel model)
    {
        VisualChanged(VisualDropdown.value);
        ModelDropdown.value = DefaultResources.Models.IndexOf(model);
    }

    public void Toggle()
    {
        View.SetActive(!View.activeInHierarchy);
        if (View.activeInHierarchy)
        {
            int i = 0;
            foreach (GameObject go in SecondScreen.Instance.Cameras)
            {
                if (screenToggles.FirstOrDefault(s=>s.Camera == go)==null)
                {
                    GameObject newScreenToggle = Instantiate(ScreenTogglePrefab);
                    newScreenToggle.transform.SetParent(ScreensHub);
                    newScreenToggle.transform.localPosition = Vector3.zero;
                    newScreenToggle.transform.localScale = Vector3.one;
                    newScreenToggle.GetComponent<ScreenToggle>().Init(go, i);
                    screenToggles.Add(newScreenToggle.GetComponent<ScreenToggle>());
                }
                i++;
            }
        }
    }

    private void VisualChanged(int v)
    {
        if (ActiveModel.Value)
        {
            if (v == 1)
            {
                Effect.visualEffectAsset = ActiveModel.Value.AnimationGraph;
            }
            else
            {
                Effect.visualEffectAsset = ActiveModel.Value.KinectGraph;
            }
        }
    }

}
