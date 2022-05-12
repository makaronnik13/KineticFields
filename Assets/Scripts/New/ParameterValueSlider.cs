using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace KineticFields.Ui
{
    public class ParameterValueSlider : MonoBehaviour
    {
        [SerializeField]
        private TwoSideSlider slider;
        [SerializeField]
        private Slider valueSlider;

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Init(ParameterInstance parameter)
        {
            disposables.Dispose();

            slider.LeftValue.Value = (parameter.UserMinValue - parameter.MinValue) / (parameter.MaxValue - parameter.MinValue);
            slider.RightValue.Value = (parameter.MaxValue - parameter.UserMaxValue) / (parameter.MaxValue - parameter.MinValue);

            slider.LeftValue.Subscribe(left=>
            {
                parameter.UserMinValue = parameter.MinValue + left * (parameter.MaxValue-parameter.MinValue);
            }).AddTo(disposables);

            slider.RightValue.Subscribe(right =>
            {
                parameter.UserMaxValue = parameter.MaxValue - right * (parameter.MaxValue - parameter.MinValue);
            }).AddTo(disposables);

            valueSlider.minValue = parameter.MinValue;
            valueSlider.maxValue = parameter.MaxValue;
            valueSlider.value = parameter.UserValue;

            
        }

        private void OnDisable()
        {
            disposables.Dispose();
        }
    }
}
