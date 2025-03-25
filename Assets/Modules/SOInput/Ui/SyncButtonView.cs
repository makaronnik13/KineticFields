using System;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Zenject;
using Klak.VJUI;

[RequireComponent(typeof(Klak.VJUI.Button))]
public class SyncButtonView : SyncControllView<float>
{
    private Klak.VJUI.Button _button;
    private bool _value;
    [Inject]

    private void Construct(KineticInputService inputService)
    {
        _button = GetComponent<Klak.VJUI.Button>();

        _button.onButtonDown.AddListener(() =>
        {
            _value = true;
        });

  
        _button.onButtonUp.AddListener(() =>
        {
            _value = false;
        });

        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                InputInstance?.SetValue(_value?1:0);
            }).AddTo(this);
            
            (InputInstance as ButtonSO).OnPressed += () =>
            {
               _button.SetPressed(true); // Кнопка "не нажата"
                Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ =>
                {
                    _button.SetPressed(false);  // Кнопка "нажата"
                }).AddTo(this);
            };
        }).AddTo(this);
    }
}
