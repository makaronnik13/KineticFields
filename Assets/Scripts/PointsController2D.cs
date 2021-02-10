using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PointsController2D : MonoBehaviour
{
    [SerializeField]
    private ModifiyngParameterView GravitySlider, SpeedSlider;

    private ModifyingParameter Gravity, Speed;
    private KineticSession session;

    public float gravityMultiplyer
    {
        get
        {
            return Gravity.Value.Value;
        }
    }
    public float globalSpeedMultiplyer
    {
        get
        {
            return Speed.Value.Value*0.25f;
        }
    }

    public static PointsController2D Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (KineticFieldController.Instance.KeysEnabled)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (KineticFieldController.Instance.ActivePoint.Value == null)
                {
                    KineticFieldController.Instance.ActivePoint.SetState(transform.GetChild(0).GetComponent<KineticPoint>());
                }
                else
                {
                    int pos = GetComponentsInChildren<KineticPoint>().ToList().IndexOf(KineticFieldController.Instance.ActivePoint.Value);

                    pos++;
                    if (pos >= GetComponentsInChildren<KineticPoint>().Count())
                    {
                        pos = 0;
                    }
                    KineticFieldController.Instance.ActivePoint.SetState(GetComponentsInChildren<KineticPoint>()[pos]);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (KineticFieldController.Instance.ActivePoint.Value == null)
                {
                    KineticFieldController.Instance.ActivePoint.SetState(transform.GetChild(0).GetComponent<KineticPoint>());
                }
                else
                {
                    int pos = GetComponentsInChildren<KineticPoint>().ToList().IndexOf(KineticFieldController.Instance.ActivePoint.Value);

                    pos--;
                    if (pos < 0)
                    {
                        pos = GetComponentsInChildren<KineticPoint>().Count() - 1;
                    }

                    KineticFieldController.Instance.ActivePoint.SetState(GetComponentsInChildren<KineticPoint>()[pos]);
                }
            }
        }
       
    }

    private void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
    }

    private void SessionChanged(KineticSession session)
    {
        if (this.session!=null)
        {
            this.session.ActivePreset.RemoveListener(PresetChanged);
        }

        session.ActivePreset.AddListener(PresetChanged);
    }

    private void PresetChanged(KineticPreset preset)
    {
        if (preset == null)
        {
            return;
        }

        preset = KineticFieldController.Instance.Session.Value.ActivePreset.Value;
        Gravity = preset.Gravity;
        Speed = preset.Speed;
        GravitySlider.Init(Gravity);
        SpeedSlider.Init(Speed);
    }
}
