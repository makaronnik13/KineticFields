using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace KineticFields.Ui
{
    public class UiRepository : MonoBehaviour
    {
        [SerializeField]
        private GameObject parameterView;

        private PrefabCreator prefabCreator;

        [Inject]
        public void Construct(PrefabCreator prefabCreator)
        {
            this.prefabCreator = prefabCreator;
        }

        public ParameterView CreateParameterView(ParameterInstance parameter, RectTransform parentRect)
        {
            return prefabCreator.Create<ParameterView>(parameterView, parentRect, new List<object>() { parameter });
        }
    }
}
