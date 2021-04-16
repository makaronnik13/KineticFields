using Assets.Scripts;
using Assets.WasapiAudio.Scripts.Unity;
using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Windows.Kinect;

public class SettingsPanel : Singleton<SettingsPanel>
{
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
    TMPro.TMP_Dropdown SourceDropdown, VisualDropdown, ModelDropdown;

    [SerializeField]
    private VisualEffect Effect;

    public GenericFlag<KineticModel> ActiveModel = new GenericFlag<KineticModel>("ActiveModel", null);
   

    // Start is called before the first frame update
    void Start()
    {
        SourceDropdown.onValueChanged.AddListener(SorceChanged);
        VisualDropdown.onValueChanged.AddListener(VisualChanged);


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

    private void SorceChanged(int v)
    {
        if (v == 0)
        {
            source.SetSourceType(true);
            Bar.Profile = MicProfile;
            Processor.Profile = MicProfile;
        }
        else
        {
            source.SetSourceType(false);
            Bar.Profile = AudioProfile;
            Processor.Profile = AudioProfile;
        }
    }
}
