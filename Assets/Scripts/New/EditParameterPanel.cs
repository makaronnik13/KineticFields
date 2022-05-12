using KineticFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using TMPro;

namespace KineticFields.Ui
{
    public class EditParameterPanel : MonoBehaviour, IShowable<ParameterInstance>
    {
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private GameObject view;
        [SerializeField]
        private TMP_Text parameterName, parameterValue;
        [SerializeField]
        private ParameterValueSlider parameterValueSlider;
        [SerializeField]
        private Toggle envelopeToggle, fftTogle;
        [SerializeField]
        private GameObject envelopeView, fftView;


        private ParameterInstance parameter;
        private CompositeDisposable disposables = new CompositeDisposable();
        private SpectrumSelector spectrumSelector;
        private EnvelopeEditor envelopeEditor;

        [Inject]
        public void Construct(SpectrumSelector spectrumSelector, EnvelopeEditor envelopeEditor)
        {
            this.spectrumSelector = spectrumSelector;
            this.envelopeEditor = envelopeEditor;

            closeBtn.OnClickAsObservable().Subscribe(_=> 
            {
                Hide();
            }).AddTo(this);

        }

        public void Hide()
        {
            disposables.Dispose();
            view.SetActive(false);
        }

        public void Show(ParameterInstance parameter)
        {
            this.parameter = parameter;
            parameterName.text = parameter.Name;
            parameter.Value.Subscribe(v=>
            {
                parameterValue.text = v.ToString();
            }).AddTo(disposables);

            parameterValueSlider.Init(parameter);

            envelopeToggle.OnValueChangedAsObservable().Subscribe(v =>
            {
                parameter.UseEnvelope.Value = v;
            }).AddTo(disposables);

            fftTogle.OnValueChangedAsObservable().Subscribe(v =>
            {
                parameter.UseFft.Value = v;
            }).AddTo(disposables);

            parameter.UseFft.Subscribe(v=>
            {
                fftView.SetActive(v);
            }).AddTo(disposables);

            parameter.UseEnvelope.Subscribe(v =>
            {
                envelopeView.SetActive(v);
            }).AddTo(disposables);


            view.SetActive(true);
        }
    }
}