using UniRx;

public interface IBPMSource
{
    public ReactiveCommand OnBeat { get; }
    public ReactiveCommand<int> OnBPMchanged { get;}
}
