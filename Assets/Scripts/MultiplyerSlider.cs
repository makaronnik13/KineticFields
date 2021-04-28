using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultiplyerSlider : MonoBehaviour
{

    [SerializeField]
    private BarSpectrum SpectrumBar;

    public float scale = 1f;



    private void Update()
    {
        float max = SpectrumBar.GetSpectrumData().ToList().Max();
        if (max*scale>0.12f)
        {
            scale = Mathf.Lerp(scale, 0, Time.deltaTime*0.3f);
        }
        else if(max*scale<0.05f)
        {
            scale = Mathf.Lerp(scale, 2, Time.deltaTime*0.3f);
        }

        SpectrumBar.AudioScale = 1000f * scale;

    }


}
