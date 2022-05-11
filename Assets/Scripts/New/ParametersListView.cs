using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

namespace KineticFields.Ui
{
    public class ParametersListView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private RectTransform root;
        [SerializeField]
        private Button showBtn;
        [SerializeField]
        private Animator animator;

        private UiRepository uiRepository;

        [Inject]
        public void Construct(UiRepository uiRepository, EditParameterPanel editParameterPanel)
        {
            this.uiRepository = uiRepository;
            showBtn.OnClickAsObservable().Subscribe(_=>
            {
                if (animator.GetBool("Show"))
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }).AddTo(this);
        }

        public void Show(List<ParameterInstance> parameters)
        {
            foreach (Transform t in root)
            {
                Destroy(t.gameObject);
            }

            foreach (ParameterInstance parameter in parameters)
            {
                CreateParameterView(parameter);
            }
        }

        public void CreateParameterView(ParameterInstance parameter)
        {
            uiRepository.CreateParameterView(parameter, root);
        }

        public void Show()
        {
            animator.SetBool("Show", true);
        }

        public void Hide()
        {
            animator.SetBool("Show", false);
        }
    }
}
