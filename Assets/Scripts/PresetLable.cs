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
    private TMPro.TextMeshProUGUI Lable;


    // Start is called before the first frame update
    void Start()
    {
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
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
            StartCoroutine(HideLable());
        }
        else
        {
            Lable.text = string.Empty;
        }
    }

    private IEnumerator HideLable()
    {
        Lable.color = Color.white;
        float t = 2f;
        while (t>0)
        {
            Lable.color = Color.Lerp(new Color(1,1,1,0), Color.white, t /2f);
            t -= Time.deltaTime;
            yield return null;
        }
    }
}
