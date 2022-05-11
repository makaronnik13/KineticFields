using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using System;

namespace KineticFields.Ui
{
    public class TwoSideSlider : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        [SerializeField]
        private RectTransform rect;
        public ReactiveProperty<float> LeftValue { get; private set; } = new ReactiveProperty<float>(0);
        public ReactiveProperty<float> RightValue { get; private set; } = new ReactiveProperty<float>(1);

        private int pressedSide = -1;
        private RectTransform mainRect;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                SetSide();
                SetValue();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                SetSide();
            }
        }

        void Update()
        {
            if (pressedSide != -1)
            {
                SetValue();
            }

            if (Input.GetMouseButtonUp(0))
            {
                pressedSide = -1;
            }
        }


        public void Start()
        {
            mainRect = transform as RectTransform;

            this.OnMouseDownAsObservable().Subscribe(_=>
            {
                Debug.Log(Input.mousePosition);
            }).AddTo(this);

            LeftValue.Subscribe(_ =>
            {
                LeftValue.Value = Mathf.Clamp(LeftValue.Value, 0, RightValue.Value);
                UpdateRect();
            }).AddTo(this);

            RightValue.Subscribe(_ =>
            {
                RightValue.Value = Mathf.Clamp(RightValue.Value, LeftValue.Value, 1);
                UpdateRect();
            }).AddTo(this);
        }

        private void UpdateRect()
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainRect.rect.width * (RightValue.Value - LeftValue.Value));
            rect.anchoredPosition = new Vector2(mainRect.rect.width*LeftValue.Value, 0);
        }

        private void SetValue()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector2 v;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePos, null, out v);
            float normalizedPosition = (v.x / mainRect.rect.width) + 0.5f;

            if (pressedSide == 0)
            {
                LeftValue.Value = normalizedPosition;
            }
            else
            {
                RightValue.Value = normalizedPosition;
            }
        }

        private void SetSide()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 v;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePos, null, out v);
            float normalizedPosition = (v.x / mainRect.rect.width) + 0.5f;

            Debug.Log(LeftValue.Value +" "+RightValue.Value+"    "+((RightValue.Value - LeftValue.Value) / 2f));

            float center = LeftValue.Value + (RightValue.Value - LeftValue.Value)/2f;

            Debug.Log(normalizedPosition+" "+center);
            if (normalizedPosition < center)
            {
                pressedSide = 0;
            }
            else
            {
                pressedSide = 1;
            }
        }

    }
}