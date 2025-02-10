using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KineticFields;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class DeviceSelector : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown dropdown;

    [Inject]
    public void Construct(FFTService fftService)
    {
        fftService.OnDeviceListChanged.Subscribe(_ =>
        {
            dropdown.options = fftService.SourceVariants.Select(s => new TMP_Dropdown.OptionData(s.Name)).ToList();
            dropdown.value = fftService.DeviceId;
        }).AddTo(this);
        
        fftService.CaptureDevice.Subscribe(device =>
        {
            dropdown.value = fftService.DeviceId;
        }).AddTo(this);

        dropdown.onValueChanged.AsObservable().Subscribe(fftService.SelectDevice).AddTo(this);
    }
}
