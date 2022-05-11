using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;
using KineticFields.Ui;

namespace KineticFields
{
    public class SceneTest : MonoBehaviour
    {
        [SerializeField]
        public ParametersListView parametersList;

        [Inject]
        void Construct(ParameterInstance.Factory parameterFactory)
        {
            List<ParameterData> datas = new List<ParameterData>();
            datas.Add(new ParameterData("TestParameter_1", -5f, 15f));
            datas.Add(new ParameterData("TestParameter_2", 0f, 1f));
            datas.Add(new ParameterData("TestParameter_3", 0f, 1f));
            datas.Add(new ParameterData("TestParameter_4", 0f, 100f));
            datas.Add(new ParameterData("TestParameter_5", 1f, 5f));

            List<ParameterInstance> parameters = new List<ParameterInstance>();

            foreach (ParameterData data in datas)
            {
                parameters.Add(parameterFactory.Create(data));
            }

            parametersList.Show(parameters);
        }
    }
}