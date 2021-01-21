using Assets.WasapiAudio.Scripts.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSourceBtn : MonoBehaviour
{
    [SerializeField]
    WasapiAudioSource source;

    [SerializeField]
    private bool fromMic = false;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite mic, sound;

    public void Toggle()
    {
        fromMic = !fromMic;

        if (fromMic)
        {
            icon.sprite = mic;
        }
        else
        {
            icon.sprite = sound;
        }

        source.SetSourceType(fromMic);
    }
}
