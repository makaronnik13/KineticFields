using System;
using UnityEngine;
using TMPro;

namespace KineticFields
{
    public class ParameterPanel
    {
        [SerializeField]
        private GameObject view;

        [SerializeField]
        private TMP_Text parameterName;

        public void ShowParameter(ParameterInstance parameter)
        {
            view.SetActive(parameter!=null);
            if (parameter == null)
            {
                return;
            }
            parameterName.text = parameter.Name;
        }
    }
}