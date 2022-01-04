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
using Zenject;
using UnityEngine.EventSystems;
using UniRx;

public class SettingsPanel : Singleton<SettingsPanel>
{

    [SerializeField]
    private GameObject ScreenTogglePrefab;

    [SerializeField]
    private Transform ScreensHub;

    [SerializeField]
    public GameObject Resolink;

    [SerializeField]
    private Toggle ResolinkToggle;

    [SerializeField]
    private BarSpectrum Bar;

    [SerializeField]
    private AudioProcessor Processor;

   

    [SerializeField]
    private GameObject View;

    [SerializeField]
    WasapiAudioSource source;

    [SerializeField]
    TMPro.TMP_Dropdown  VisualDropdown, ModelDropdown, SoundSourceDropdown;

    [SerializeField]
    private VisualEffect Effect;

    private SoundSourceProvider soundSourceProvider;

    public GenericFlag<KineticModel> ActiveModel = new GenericFlag<KineticModel>("ActiveModel", null);

    

    public Action<bool> OnResolinkStateChanged = (v) => { };

    private List<ScreenToggle> screenToggles = new List<ScreenToggle>();


    [Inject]
    private void Construct(SoundSourceProvider soundSourceProvider)
    {
        this.soundSourceProvider = soundSourceProvider;

        soundSourceProvider.Devices.ObserveCountChanged().Subscribe(v =>
        {
            SoundSourcesChanged();
        });

        soundSourceProvider.Devices.ObserveAdd().Subscribe(x =>
        {
            SoundSourcesChanged();
        });

        soundSourceProvider.Devices.ObserveRemove().Subscribe(x =>
        {
            SoundSourcesChanged();
        });
    }

    private void SoundSourcesChanged()
    {
        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
        options.Add(new TMPro.TMP_Dropdown.OptionData("System sound"));

        foreach (MMDevice device in soundSourceProvider.Devices)
        {
            options.Add(new TMPro.TMP_Dropdown.OptionData(device.FriendlyName.Split('(', ')')[1]));
        }

        SoundSourceDropdown.ClearOptions();
        SoundSourceDropdown.AddOptions(options);

        if (soundSourceProvider.ActiveSoundDevice.Value == null || options.FirstOrDefault(o => o.text == soundSourceProvider.ActiveSoundDevice.Value.FriendlyName.Split('(', ')')[1]) == null)
        {
            if (options.Count == 0)
            {
                SoundSourceDropdown.value = 0;
            }
            else
            {
                SoundSourceDropdown.value = soundSourceProvider.Devices.IndexOf(soundSourceProvider.GetDefaultDevice()) + 1;
            }
        }

        SoundSourceDropdown.RefreshShownValue();
    }


    // Start is called before the first frame update
    void Start()
    {
        VisualDropdown.onValueChanged.AddListener(VisualChanged);
        SoundSourceDropdown.onValueChanged.AddListener(SoundDropdownChanged);

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


    

        ResolinkToggle.onValueChanged.AddListener(ToggleResolink);
    }

    private void SoundDropdownChanged(int value)
    {
        soundSourceProvider.SelectSoundSource(value);
    }

    private void ToggleResolink(bool v)
    {
        Resolink.gameObject.SetActive(v);
        OnResolinkStateChanged(v);
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
