using System;
using System.Collections;
using System.Collections.Generic;
using KinectVfx;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DepthSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text value;
    
    [Inject]
    public void Construct(KinectPointCloud kinectPointCloud)
    {
        if (kinectPointCloud==null)
        {
            Debug.LogWarning("Depth slider has no reference to kinekt point cloud");
            return;
        }
        slider.value = kinectPointCloud.maxDepth;
        value.text = kinectPointCloud.maxDepth.ToString();
        
        slider.onValueChanged.AsObservable().Subscribe(v =>
        {
            kinectPointCloud.maxDepth = v;
            value.text = String.Format("{0:0.#}", v);
        }).AddTo(this);
    }
}
