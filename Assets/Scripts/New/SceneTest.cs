using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace KineticFields
{
    public class SceneTest : MonoBehaviour
    {
        private ParameterInstance parameter;

        [SerializeField]
        private ParameterPanel ParameterPanel;

        [Inject]
        void Construct(ParameterInstance.Factory parameterFactory)
        {
            ParameterData newData = new ParameterData("TestParameter", -5f, 15f);
            parameter = parameterFactory.Create(newData);
        }
    }
}