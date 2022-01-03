using Assets.WasapiAudio.Scripts.Unity;
using Assets.WasapiAudio.Scripts.Wasapi;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AudioDecorator
{
    public ReactiveProperty<WasapiCaptureType> CaptureType = new ReactiveProperty<WasapiCaptureType>(WasapiCaptureType.Loopback);

    private float[] chachedSpectrum = null;
    public float[] Spectrum
    {
        get
        {
            if (chachedSpectrum == null || chachedSpectrum.Length == 0)
            {
                AnalyzeSpectrum();
            }
            return chachedSpectrum;
        }
    }

    private void AnalyzeSpectrum()
    {
        //chachedSpectrum = audioProcessor.get
    }

    private WasapiAudioSource audioSource;
    private AudioProcessor audioProcessor;

    [Zenject]
    public void Construct(WasapiAudioSource audioSource, AudioProcessor audioProcessor)
    {
        this.audioSource = audioSource;
        this.audioProcessor = audioProcessor;

        Debug.Log("construct decorator");
        Debug.Log("audio source "+audioSource);


    }
}
