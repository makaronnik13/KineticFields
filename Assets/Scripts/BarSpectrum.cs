﻿using System;
using System.Collections;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class BarSpectrum : AudioVisualizationEffect
    {
        private GameObject[] _spectrumBars;
        private Vector3[] _originalPositions;
        private Vector3 _originalScale;
        private float[] _spectrumCaps;
        private float[] _spectrumBarsSensivity;



        public GameObject Prefab;
        public float AudioScale;
        public float Power;

        [Inject]
        public void Construct(AudioProcessor processor)
        {
            _spectrumBars = new GameObject[SpectrumSize];
            _originalPositions = new Vector3[SpectrumSize];
            _originalScale = Prefab.transform.localScale;

            var width = Prefab.transform.localScale.x;

            for (var i = 0; i < SpectrumSize; i++)
            {
                var spectrumBar = GameObject.Instantiate(Prefab);
                spectrumBar.transform.parent = transform;
                spectrumBar.transform.localPosition = new Vector3(width * i, 0.0f, 0.0f);
                _spectrumBars[i] = spectrumBar;
                _originalPositions[i] = spectrumBar.transform.localPosition;
            }

            Prefab.SetActive(false);


            _spectrumCaps = GetSpectrumData();
        }

        public void Update()
        {
            var spectrumData = GetSpectrumData();

            if (_spectrumCaps == null)
            {
                return;
            }

                for (var i = 0; i < SpectrumSize; i++)
                {
                    if (_spectrumCaps[i] < spectrumData[i])
                    {
                        _spectrumCaps[i] = spectrumData[i];
                    }
                    else
                    {
                        _spectrumCaps[i] -= Time.deltaTime;
                    }
                    var audioScale = Mathf.Pow(spectrumData[i] * AudioScale, Power);
                    var newScale = new Vector3(_originalScale.x, _originalScale.y + audioScale, _originalScale.z);
                    var halfScale = newScale / 2.0f;
                    _spectrumBars[i].transform.localPosition = new Vector3(_originalPositions[i].x + halfScale.x, _originalPositions[i].y + halfScale.y, _originalPositions[i].z + halfScale.z);
                    _spectrumBars[i].transform.localScale = newScale;

                    newScale = new Vector3(_originalScale.x, _originalScale.y + audioScale, _originalScale.z);

                    //_spectrumBars[i].transform.GetChild(0).localPosition = Vector3.zero;
                    //_spectrumBars[i].transform.GetChild(0).localScale = newScale;
                }
        }
    }
}
