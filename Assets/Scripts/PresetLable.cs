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

    [SerializeField]
    private GameObject PlayIcon;


    // Start is called before the first frame update
    void Start()
    {
        BpmManager.Playing.AddListener(PlayingStateChanged);
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
    }


    private void PlayingStateChanged(bool v)
    {
        PlayIcon.SetActive(v);
    }



    private void SessionChanged(KineticSession session)
    {
        Debug.Log("add listener");

        if (session!=null)
        {
            session.ActivePreset.AddListener(PresetChanged);
        }
    }

    private void PresetChanged(KineticPreset preset)
    {
        Lable.text = preset.PresetName;
        StartCoroutine(HideLable());
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
