using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class AnimatorSyncroniser : MonoBehaviour
{
    [SerializeField]
    private Animator animator;


    private ConstantBPMSource bPMSource;
    private int neededTrack = -1;
    private int bpm = 120;

    [Inject]
    public void Construct(ConstantBPMSource bpmSource)
    {
        Debug.Log(bpmSource);
        this.bPMSource = bpmSource;
        
        bpmSource.OnBeat.Subscribe(_ =>
        {
            if (neededTrack!=-1)
            {
                Debug.Log("track " + neededTrack);
                animator.SetInteger("Track", neededTrack);
                neededTrack = -1;
                SetSpeed();
            }
        }).AddTo(this);

        bpm = bpmSource.Bpm;
        bpmSource.OnBPMchanged.Subscribe(_ =>
        {
            bpm = bpmSource.Bpm;
        }).AddTo(this);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetTrack(0);

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetTrack(1);
        }
    }

    public void SetTrack(int i)
    {
        Debug.Log("set "+i);
        neededTrack = i;
    }

    public void SetSpeed()
    {
        //10 сек - длительность анимации
        //120 bpm
        //0.5 сек - 1 бит
        //
        //bpm = 10/0.5 - 20 битов

        int beatsCount = Mathf.RoundToInt(animator.GetCurrentAnimatorStateInfo(0).length / (60f / bpm));

        float length = beatsCount * (60f / bpm);
        float diff = length - animator.GetCurrentAnimatorStateInfo(0).length;
        
        animator.speed = 1f+Mathf.Abs(diff/animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
