using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ConstantBPMSource : MonoBehaviour, IBPMSource
{
    private ReactiveCommand onBeat = new ReactiveCommand();
    private ReactiveCommand<int> onBPMChanged = new ReactiveCommand<int>();
    public ReactiveCommand OnBeat => onBeat;
    public ReactiveCommand<int> OnBPMchanged => onBPMChanged;

    public int Bpm => bpm;

    private CompositeDisposable counter = new CompositeDisposable();
    
    [SerializeField] private int bpm = 120;


    void Start()
    {
        Restart(bpm);    
    }

    [ContextMenu("Restart")]
    public void Restart()
    {
        Restart(bpm);
    }
    
    public void Restart(int bpm)
    {
        this.bpm = bpm;
        counter.Clear();
        OnBPMchanged.Execute(bpm);
        Observable.Interval(TimeSpan.FromSeconds(60f * 1f / bpm)).Subscribe(_ =>
        {
            onBeat.Execute();
        }).AddTo(counter);
    }

}
