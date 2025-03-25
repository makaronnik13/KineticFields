using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


public class KineticInputService : MonoBehaviour
{
    public List<InputSO> InputInstances { get; private set; } = new List<InputSO>();
    public ReactiveCommand OnItemsLoaded = new ReactiveCommand();
    
    [SerializeField] private bool keyboardInput = true;
    
    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        InputInstances.Clear();

        var inputSOs = Resources.LoadAll<InputSO>(""); 
        foreach (var so in inputSOs)
        {
            InputSO instance = so.CreateInstance(so);

            if (keyboardInput)
            {
                Debug.Log(instance);
                Debug.Log(instance is ButtonSO);
                if (instance is ButtonSO buttonSO)
                {
                    buttonSO.Action.Enable();
                    buttonSO.Action.performed += _ =>
                    {
                        buttonSO.OnPressed?.Invoke();    
                    };
                }
            }
            
            InputInstances.Add(instance);
        }

        OnItemsLoaded.Execute();

        if (Application.isEditor)
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                ProcessInput();
            }).AddTo(this);
        }
    }

    private void ProcessInput()
    {
        if (!keyboardInput)
        {
            return;
        }
        
        // Обрабатываем нажатия клавиш с использованием новой системы ввода
        foreach (var input in InputInstances)
        {
            if (input is ToggleSO toggleSO)
            {
                toggleSO.SetValue(toggleSO.Action.ReadValue<float>());
            }
            
            if (input is KnobSO knobSO)
            {
                /*
                // Проверяем, если клавиша положительного или отрицательного действия нажата
                if (knobSO.Action.зщы)
                {
                    knobSO.SetValue(knobSO.Value.Value + Time.deltaTime);
                }

                if (knobSO.NegativeAction.IsPressed())
                {
                    knobSO.SetValue(knobSO.Value.Value - Time.deltaTime);
                }*/
            }
        }
        
    }

    public InputSO GetItemInstance(InputSO so)
    {
        return InputInstances.FirstOrDefault(i => i.UniqueId == so.UniqueId);
    }
    
    public InputSO GetItemInstance(string oscAddres)
    {
        return InputInstances.FirstOrDefault(i => i.OscName == oscAddres);
    }
}
