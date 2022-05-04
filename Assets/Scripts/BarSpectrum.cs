using System;
using System.Collections;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;

namespace KineticFields
{
    public class BarSpectrum : MonoBehaviour
    {

        private GameObject[] _spectrumBars;
        private int spectrumSize;
        private float height;
        public GameObject Prefab;

        [Inject]
        public void Construct(FFTService fftsource, PrefabCreator prefabCreator)
        {
            height = (transform as RectTransform).rect.height;
            spectrumSize = fftsource.SpectrumSize;
            _spectrumBars = new GameObject[spectrumSize];
            var width = Prefab.transform.localScale.x;

            for (var i = 0; i < spectrumSize; i++)
            {
                _spectrumBars[i] = prefabCreator.Create(Prefab, transform);
            }

            Prefab.SetActive(false);

            fftsource.OutputSpectrum.Subscribe(spectrum =>
            {
                if (spectrum==null || spectrum.Count==0)
                {
                    return;
                }

                for (var i = 0; i < spectrumSize; i++)
                {
                    var audioScale = spectrum[i];
                    (_spectrumBars[i].transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, audioScale*height);
                }
            }).AddTo(this);
        }
    }
}
