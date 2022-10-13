using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using KineticFields;

public class TextureFromSpectrum : MonoBehaviour
{
    public MeshRenderer rend;


    [Inject]
    public void Construct(FFTService fftService)
    {
        fftService.OutputSpectrum.Subscribe(spectrum =>
        {
            if (spectrum!=null)
            {
                UpdateTexture(spectrum);
            }
        }).AddTo(this);

        /*
        audioProcessor.onSpectrum.Subscribe(spectrum =>
        {
            UpdateTexture(spectrum);
        }).AddTo(this);*/
    }

    private void UpdateTexture(List<float> spectrum)
    {
        if (rend.sharedMaterial.GetTexture("MainTex") == null || rend.sharedMaterial.GetTexture("MainTex").width!=spectrum.Count)
        {
            Texture2D texture = new Texture2D(spectrum.Count, spectrum.Count);
            rend.sharedMaterial.SetTexture("MainTex", texture);
        }
        float max = spectrum.ToList().Max();

        Color[] colors = new Color[spectrum.Count*spectrum.Count];
        for (int i = 0; i < spectrum.Count; i++)
        {
            for (int j = 0; j < spectrum.Count; j++)
            {
                colors[i + j * spectrum.Count] = Color.white * (spectrum[i] * spectrum[j])/(max*max);
            }
        }


        (rend.sharedMaterial.GetTexture("MainTex") as Texture2D).SetPixels(0,0,spectrum.Count, spectrum.Count, colors);
        (rend.sharedMaterial.GetTexture("MainTex") as Texture2D).Apply();
    }
}
