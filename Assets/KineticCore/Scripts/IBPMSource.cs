using UniRx;

public interface IBPMSource
{
    public int Bpm { get; }
    public ReactiveCommand OnBeat { get; }
    public ReactiveCommand<int> OnBPMchanged { get;}
}
