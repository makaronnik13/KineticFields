using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KineticFields;
using Lasp;
using UniRx;
using UnityEngine;
using Zenject;

public class SOscilatorSource : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] public float multiplyer = 1;
    [SerializeField] private float interpolaionTime;
    
    [SerializeReference] PropertyBinder[] propertyBinders = null;
    public PropertyBinder[] PropertyBinders
    { get => (PropertyBinder[])propertyBinders.Clone();
        set => propertyBinders = value; }

    private float time; 
    public float Value => value;
    public float MultipliedValue => Value * multiplyer;
    
    private float value;

    private FFTService fftService;


    
    [Inject]
    public void Construct(FFTService fftService)
    {
        this.fftService = fftService;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            
   
                if (propertyBinders != null)
                    foreach (var b in propertyBinders) b.Level = value;
        }).AddTo(this);
    }
    

}
