using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Zenject;
using Klak.VJUI;

[RequireComponent(typeof(Klak.VJUI.Toggle))]
public class SyncToggleView : SyncControllView<float>
{
    private Klak.VJUI.Toggle _toggle;
    private bool _value;

    [Inject]

    private void Construct(KineticInputService inputService)
    {
        _toggle = GetComponent<Klak.VJUI.Toggle>();

      
        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            if (InputInstance != null)
            {
                _toggle.isOn = Mathf.Approximately(InputInstance.Value.Value, 1f);
                
                _toggle.onValueChanged.AddListener((v) =>
                { 
                    InputInstance.Value.Value = v?1:0;
                });

                InputInstance.Value.Subscribe(v =>
                {
                    _toggle.isOn = Mathf.Approximately(v, 1f);
                }).AddTo(this);
                /*
                InputInstance.Value.Subscribe(v =>
                {
                    if (Mathf.Approximately(v, 0))
                    {
                        _toggle.isOn = false;
                    }
                    else
                    {
                        _toggle.isOn = true;
                    }
                }).AddTo(this);
                */
            }


        }).AddTo(this);
    }
}
