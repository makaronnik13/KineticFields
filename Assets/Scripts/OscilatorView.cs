using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OscilatorView : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI RateValue;

    [SerializeField]
    private TMPro.TMP_InputField Multiplyer;

    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private Image Icon, CurveImg;

    [SerializeField]
    private TMPro.TextMeshProUGUI Value;

    [SerializeField]
    private RectTransform Line, Handle; 


    private Oscilator oscilator;

    private void Start()
    {
        Multiplyer.onEndEdit.AddListener(MultiplyerChanged);
    }

    private void MultiplyerChanged(string mult)
    {
        oscilator.Multiplyer = Mathf.Clamp(float.Parse(mult), -100, 100);
        Multiplyer.text = oscilator.Multiplyer.ToString();
    }

    public void Init(Oscilator oscilator)
    {
        if (this.oscilator!=null)
        {
            this.oscilator.OnValueChanged -= ValueChanged;
            this.oscilator.onTimeChanged -= TimeChanged;
            this.oscilator.onMiddleValueChanged -= MiddleValueChanged;
        }
        this.oscilator = oscilator;
        Icon.sprite = oscilator.Icon;
        oscilator.OnValueChanged += ValueChanged;
        this.oscilator.onTimeChanged += TimeChanged;
        this.oscilator.onMiddleValueChanged += MiddleValueChanged;

        Multiplyer.text = oscilator.Multiplyer.ToString();
        RateValue.text = "1/" + Mathf.Pow(2, oscilator.RepeatRate);

        CurveImg.sprite = CurveEditor.Instance.MakeScreenshot(oscilator.Curve.Curve);
    }

    private void MiddleValueChanged(float v)
    {
        Handle.anchoredPosition = new Vector2(0, (Handle.parent as RectTransform).rect.height/2f*v);
    }

    private void TimeChanged(float v)
    {
        Line.anchoredPosition = new Vector2((Line.parent as RectTransform).rect.width*v ,0);
    }

    private void ValueChanged(float v)
    {
        v = (float)Math.Round(v, 2);
        Value.text = v.ToString();
    }

    public void SelectCurve()
    {
        CurvePickWindow.Instance.Show(oscilator.Curve.Id, (id)=>
        {
            oscilator.Curve = SessionsManipulator.Instance.Curves.GetCurve(id);
            CurveImg.sprite = CurveEditor.Instance.MakeScreenshot(oscilator.Curve.Curve);
        });
    }

    public void ClickRate()
    {
        oscilator.RepeatRate++;

        if (oscilator.RepeatRate>4)
        {
            oscilator.RepeatRate = 0;
        }

        RateValue.text = "1/" + Mathf.Pow(2, oscilator.RepeatRate);
    }

    public void Toggle()
    {
        Debug.Log("toggle");
        Animator.SetBool("Show", !Animator.GetBool("Show"));

        if (Animator.GetBool("Show"))
        {
            foreach (OscilatorView osc in FindObjectsOfType<OscilatorView>())
            {
                if (osc!=this)
                {
                    osc.SetState(false);
                }
            }
        }
      
    }

    private void SetState(bool v)
    {
        Animator.SetBool("Show", v);
    }
}
