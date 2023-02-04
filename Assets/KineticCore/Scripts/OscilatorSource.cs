using KineticFields;
using Lasp;
using UniRx;
using UnityEngine;
using Zenject;

public class OscilatorSource : BaseSignalSource
{
    [SerializeField] private AnimationCurve curve;
    public AnimationCurve Curve => curve;
 

    [SerializeReference] PropertyBinder[] propertyBinders = null;
    public PropertyBinder[] PropertyBinders
    { get => (PropertyBinder[])propertyBinders.Clone();
        set => propertyBinders = value; }
    
    

    private CompositeDisposable disposables = new CompositeDisposable();
    private IBPMSource bpmSource;
    private int skipedBeats = 0;
    private float time = 0;
    private int bpm;
    public float Time => time;


    [Inject]
    public void Construct(ConstantBPMSource  bpmSource)
    {
        
        this.bpmSource = bpmSource;
        bpmSource.OnBPMchanged.Subscribe(bpm =>
        {
            this.bpm = bpm;
            
            disposables.Clear();

            skipedBeats = 0;
            time = 0;
            
            Observable.EveryUpdate().Subscribe(_ =>
            {
                time += UnityEngine.Time.deltaTime*(bpm/60f);

                if (time>= curve.keys[curve.keys.Length - 1].time)
                {
                    time = 0;
                    skipedBeats = 0;
                }


                Signal.Value = curve.Evaluate(time);
               
            }).AddTo(disposables);
            
            bpmSource.OnBeat.Subscribe(_ =>
            {
                /*
                skipedBeats += 1;
                if (isActiveAndEnabled)
                {
                    Debug.Log(skipedBeats + " " + gameObject);
             
                }
                
                if (skipedBeats>curve.keys[curve.keys.Length-1].time)
                {
                    if (isActiveAndEnabled)
                    {
                        Debug.Log("reset");
                    }
                    time = 0;
                    skipedBeats = 0;
                }
                */
            }).AddTo(disposables);
        }).AddTo(this);

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

       
            if (propertyBinders != null)
                    foreach (var b in propertyBinders) b.Level = MultipliedSignal.Value;
        }).AddTo(this);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
