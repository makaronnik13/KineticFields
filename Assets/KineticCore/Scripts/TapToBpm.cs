using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class TapToBpm : MonoBehaviour
{

    [SerializeField]
    private ConstantBPMSource bpmSource;


    [SerializeField]
    private float silenceTime;

    private CompositeDisposable relax = new CompositeDisposable();

    private float lastClick;
    private List<float> dists = new List<float>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
                if (lastClick==0)
                {
                    lastClick = Time.realtimeSinceStartup;
               
                }
            else
            {
                float d = Time.realtimeSinceStartup - lastClick;
                Debug.Log(d);
                dists.Add(d);
            }


                relax.Clear();
                Observable.Timer(TimeSpan.FromSeconds(silenceTime)).Subscribe(_ =>
                {
                    if (dists.Count == 0)
                    {
                        return;
                    }
                    Debug.Log(dists.Average());
                    Debug.Log("set "+ Mathf.RoundToInt(60f/dists.Average()));

                    int bpm = Mathf.RoundToInt(60f / dists.Average());

                    float changeTime = lastClick;
                    while (changeTime<=Time.realtimeSinceStartup)
                    {
                        changeTime += 60f / bpm;
                    }

                    Observable.Timer(TimeSpan.FromSeconds(changeTime-Time.realtimeSinceStartup)).Subscribe(_ =>
                    {
                        bpmSource.Restart(bpm);
                        lastClick = 0;
                        dists.Clear();
                    }).AddTo(this);
                }).AddTo(relax);

            lastClick = Time.realtimeSinceStartup;

        }
    }
}
