using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace KineticFields.Ui
{

    public class SpectrumSelector : MonoBehaviour
    {
        [SerializeField]
        private TwoSideSlider slider;

        [SerializeField]
        private RectTransform line;


        private CompositeDisposable disposables = new CompositeDisposable();


        public void Show(RectTransform rect, Action<float, float> onGapChanged, ReactiveProperty<float> linePosition)
        {
            (transform as RectTransform).offsetMin = new Vector2(0, 0);
            (transform as RectTransform).offsetMax = new Vector2(0, 0);

            float height = (transform as RectTransform).rect.height;

            disposables.Dispose();

            slider.LeftValue.Subscribe(left=>
            {
                onGapChanged.Invoke(slider.LeftValue.Value, slider.RightValue.Value);
            }).AddTo(disposables);

            slider.RightValue.Subscribe(right=>
            {
                onGapChanged.Invoke(slider.LeftValue.Value, slider.RightValue.Value);
            }).AddTo(disposables);

            linePosition.Subscribe(v=>
            {
                line.anchoredPosition = new Vector2(line.anchoredPosition.x, height*v);
            }).AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Dispose();
        }
    }
}
