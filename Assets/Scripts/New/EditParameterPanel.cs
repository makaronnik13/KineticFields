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

        private CompositeDisposable disposables = new CompositeDisposable();

        [Inject]
        public void Construct()
        {
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
            parameterName.text = parameter.Name;
            parameter.Value.Subscribe(v=>
            {
                parameterValue.text = v.ToString();
            }).AddTo(disposables);

            view.SetActive(true);
        }
    }
}