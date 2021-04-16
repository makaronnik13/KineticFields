using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetLable : MonoBehaviour
{
    [SerializeField]
    private BpmManager BpmManager;

    [SerializeField]
    private PresetSquare Sqare;

    [SerializeField]
    private TMPro.TMP_InputField Lable;
    

    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
        Lable.onValueChanged.AddListener(PresetNameChanged);
        PresetsLerper.Instance.Lerping.AddListener(LerpingChanged);
    }

    private void LerpingChanged(bool v)
    {
        gameObject.SetActive(!v);
        if (KineticFieldController.Instance.Session.Value!=null)
        {
            PresetChanged(KineticFieldController.Instance.Session.Value.ActivePreset.Value);
        }
      
    }

    private void PresetNameChanged(string pName)
    {
        KineticFieldController.Instance.Session.Value.ActivePreset.Value.PresetName = pName;
        SessionsManipulator.Instance.Autosave();
    }

    private void SessionChanged(KineticSession session)
    {
        if (session!=null)
        {
            session.ActivePreset.AddListener(PresetChanged);
        }
    }

    private void PresetChanged(KineticPreset preset)
    {
        if (preset!=null)
        {
            Lable.text = preset.PresetName;
        }
        else
        {
            Lable.text = string.Empty;
        }
        Sqare.Init(preset);
    }
}
