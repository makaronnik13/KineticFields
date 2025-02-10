using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class AnimatorSyncroniser : MonoBehaviour
{
    [SerializeField]
    private Animation animation;

    private KineticCoreInput input;


    private ConstantBPMSource bPMSource;

    public int neededTrack = 0;

    private int bpm = 120;

    [SerializeField]
    private List<AnimationClip> clips = new List<AnimationClip>();


    [Inject]
    public void Construct(ConstantBPMSource bpmSource)
    {
        Debug.Log(bpmSource);
        this.bPMSource = bpmSource;

        input = new KineticCoreInput();

        input.Controllers.LeftClip.performed += (context) =>
        {
           
                Debug.Log("!");
                neededTrack--;
                if (neededTrack < 0)
                {
                    neededTrack = clips.Count - 1;
                }

                SetTrack(neededTrack);
         
        };

        if (input.Controllers.LeftClip.IsPressed())
        {
            Debug.Log("!");
            neededTrack--;
            if (neededTrack < 0)
            {
                neededTrack = clips.Count - 1;
            }

            SetTrack(neededTrack);
        }

        if (input.Controllers.RightClip.IsPressed())
        {
            neededTrack++;
            if (neededTrack >= clips.Count)
            {
                neededTrack = 0;
            }

            SetTrack(neededTrack);
        }




        bpmSource.OnBeat.Subscribe(_ =>
        {
                SetSpeed();

        }).AddTo(this);

        bpm = bpmSource.Bpm;
        bpmSource.OnBPMchanged.Subscribe(_ =>
        {
            bpm = bpmSource.Bpm;
        }).AddTo(this);

        animation.Stop();
        foreach (AnimationClip clip in clips)
        {
            animation.AddClip(clip, clip.name);
            animation.Play();
        }

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
        //Debug.Log(i);
        neededTrack = i;
        animation.Play(clips[i].name);
    }

    public void SetSpeed()
    {
        //10 сек - длительность анимации
        //120 bpm
        //0.5 сек - 1 бит
        //
        //bpm = 10/0.5 - 20 битов
        foreach (AnimationState state in animation)
        {
            int beatsCount = Mathf.RoundToInt(state.length / (60f / bpm));

            float length = beatsCount * (60f / bpm);
            float diff = length - state.length;

            state.speed = 1f + Mathf.Abs(diff / state.length);

            //state.speed = 0.5F;
        }

        SetTrack(neededTrack);
    }
}
