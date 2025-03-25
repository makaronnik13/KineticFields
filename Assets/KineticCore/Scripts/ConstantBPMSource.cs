using System;
using UniRx;
using UnityEngine;

public class ConstantBPMSource : MonoBehaviour
{
    public ReactiveCommand OnBeat {get; private set; }  = new ReactiveCommand();
    public ReactiveCommand OnResync {get; private set; }  = new ReactiveCommand();
    public ReactiveProperty<int> Bpm {get; private set; }  = new ReactiveProperty<int>();
    
    private CompositeDisposable counter = new CompositeDisposable();
    
    private readonly int _startBpm = 120;


    void Start()
    {
        Restart(_startBpm);    
    }

    [ContextMenu("Restart")]
    public void Restart()
    {
        Restart(Bpm.Value);
    }
    
    public void Restart(int bpm)
    {
        Bpm.Value = bpm;
        counter.Clear();
        Observable.Interval(TimeSpan.FromSeconds(60f * 1f / bpm)).Subscribe(_ =>
        {
            OnBeat.Execute();
        }).AddTo(counter);
        OnResync.Execute();
    }

    void OnDestroy()
    {
        counter.Dispose();
    }
}
