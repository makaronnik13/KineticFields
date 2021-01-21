using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyGapSlider : MonoBehaviour
{
    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Image Selector;

    [SerializeField]
    private Slider Slider;

    private FrequencyGap gap;

    private void Start()
    {
        FindObjectOfType<KineticFieldController>().ActiveGap.AddListener(ActiveGapChanged);
        Slider.onValueChanged.AddListener(SliderValueChanged);
    }

    private void Update()
    {
        if (FindObjectOfType<KineticFieldController>().ActiveGap.Value == gap)
        {
            gap.GapSize.SetState(Mathf.Clamp(gap.GapSize.Value+Time.deltaTime*Input.mouseScrollDelta.y ,0.01f,1f));
        }
    }


    private void SizeSliderValueChanged(float v)
    {
        gap.GapSize.SetState(v);
    }

    private void SliderValueChanged(float v)
    {
        gap.Position.SetState(v);
        FindObjectOfType<KineticFieldController>().ActiveGap.SetState(gap);
    }

    private void ActiveGapChanged(FrequencyGap fg)
    {
        Selector.enabled = (fg == gap);
    }

    public void Init(FrequencyGap gap)
    {
        this.gap = gap;


        gap.Position.AddListener(GapChanged);
        gap.GapSize.AddListener(GapChanged);
        Icon.sprite = gap.Sprite;
        Icon.color = gap.color;
        Selector.color = gap.color;
        GapChanged(gap.Position.Value);
    }

    private void GapChanged(float v)
    {
        Slider.value = gap.Position.Value;
 
    }

    public void PushGapIcon()
    {
        FindObjectOfType<KineticFieldController>().ActiveGap.SetState(gap);
    }

    private void OnDestroy()
    {
        if (FindObjectOfType<KineticFieldController>())
        {
            FindObjectOfType<KineticFieldController>().ActiveGap.RemoveListener(ActiveGapChanged);
        }
       
    }
}
