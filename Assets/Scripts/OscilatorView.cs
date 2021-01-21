using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OscilatorView : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve Curve;

    void Start()
    {
        //Init(new Oscilator("O1", Curve, 2));    
    }

    /*
    private void Init(Oscilator oscilator)
    {
        AnimationCurve curve = DefaultResources.Settings.SizeCurves[oscilator.CurveId]; 
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
