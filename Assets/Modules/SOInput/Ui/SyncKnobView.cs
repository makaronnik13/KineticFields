using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Zenject;
using Klak.VJUI;

[RequireComponent(typeof(Klak.VJUI.Knob))]
public class SyncKnobView : SyncControllView<float>
{
    private Klak.VJUI.Knob _knob;

    [Inject]
    private void Construct(KineticInputService inputService)
    {
        _knob = GetComponent<Klak.VJUI.Knob>();

        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            if (InputInstance != null)
            {
                _knob.value = InputInstance.Value.Value;
                _knob.onValueChanged.AddListener((v) =>
                {

                    //Debug.Log(InputInstance.GetInstanceID() + "-" + InputInstance.UniqueId + " " + InputInstance.name + " = " + InputInstance.Value);
                    InputInstance.Value.Value = v;

                });

                InputInstance.Value.Subscribe(v =>
                {
                    _knob.value = v;
                }).AddTo(this);
            }
        }).AddTo(this);
    }
}

