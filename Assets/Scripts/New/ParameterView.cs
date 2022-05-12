using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

namespace KineticFields.Ui
{
    public class ParameterView : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private TMP_Text parameterName, value;

        [SerializeField]
        private Button btn;


        private ParameterInstance parameter;

        [Inject]
        public void Construct(ParameterInstance parameter, EditParameterPanel editPanel)
        {
            this.parameter = parameter;

            parameterName.text = parameter.Name;

            slider.minValue = parameter.MinValue;
            slider.maxValue = parameter.MaxValue;

            parameter.Value.Subscribe(v=> 
            {
                slider.value = v;
                value.text = v.ToString();
            }).AddTo(this);

            btn.OnClickAsObservable().Subscribe(_=> 
            {
                editPanel.Show(parameter);
            });
        }
    }
}
