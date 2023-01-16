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

    private CompositeDisposable counter = new CompositeDisposable();
    
    [SerializeField] private int BPM = 120;

    void Start()
    {
        Restart(BPM);    
    }

    [ContextMenu("Restart")]
    public void Restart()
    {
        Restart(BPM);
    }
    
    public void Restart(int bpm)
    {
        BPM = bpm;
        counter.Clear();
        OnBPMchanged.Execute(bpm);
        Observable.Interval(TimeSpan.FromSeconds(60f * 1f / BPM)).Subscribe(_ =>
        {
            onBeat.Execute();
        }).AddTo(counter);
    }
}
