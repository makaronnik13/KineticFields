using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArcSlider : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler, IEndDragHandler
{
    public float DistMultiplyer = 1;

    [SerializeField]
    private Image FillCircle, FillIcon, IconBack;
    [SerializeField]
    private TMPro.TextMeshProUGUI ValueChanger;

    public Action<float> OnValueChanged = (v) =>{};

    private Vector2 startMousePos;
    private float startValue;

    private float sliderValue = 0;
    public float Value
    {
        get
        {
            return sliderValue;
        }
        set
        {

            float v = Mathf.Clamp(value, -1, 1);

            if (sliderValue!=v)
            {
                sliderValue = v;
                OnValueChanged(v);

                if (v<0)
                {
                    FillIcon.fillAmount = -v;
                    FillCircle.fillAmount = -v;
                    FillIcon.color = Color.red;
                    FillCircle.color = Color.red;
                }
                else
                {
                    FillIcon.fillAmount = v;
                    FillCircle.fillAmount = v;
                    FillIcon.color = Color.green;
                    FillCircle.color = Color.green;
                }
                

                ValueChanger.enabled = true;
                ValueChanger.text = System.Math.Round(v, 1).ToString();
                
            }
        }
    }


    public void Init(Sprite icon)
    {
        FillIcon.sprite = icon;
        IconBack.sprite = icon;
    }

    private void Start()
    {
        ValueChanger.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startValue = Value;
        startMousePos = Input.mousePosition;
        ValueChanger.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 diff = -startMousePos + (Vector2)Input.mousePosition;

        Value = startValue + (diff.x+diff.y)*DistMultiplyer;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<ModifiyngParameterView>().ChooseSource();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ValueChanger.enabled = false;
    }
}