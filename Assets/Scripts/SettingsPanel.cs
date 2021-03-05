using Assets.WasapiAudio.Scripts.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Windows.Kinect;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    WasapiAudioSource source;

    [SerializeField]
    TMPro.TMP_Dropdown SourceDropdown, VisualDropdown;

    [SerializeField]
    private VisualEffect Effect;

    [SerializeField]
    private VisualEffectAsset KinnectAsset, MeshAsset;


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
    }

    public void Toggle()
    {
        View.SetActive(!View.activeInHierarchy);
    }

    private void VisualChanged(int v)
    {
        if (v==1)
        {
            Debug.Log("mesh");
            Effect.visualEffectAsset = MeshAsset;
        }
        else
        {
            Debug.Log("kinekt");
            Effect.visualEffectAsset = KinnectAsset;
        }
    }

    private void SorceChanged(int v)
    {
        if (v == 0)
        {
            source.SetSourceType(true);
        }
        else
        {
            source.SetSourceType(false);
        }
    }
}
