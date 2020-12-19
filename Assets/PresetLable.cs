using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetLable : MonoBehaviour
{
    [SerializeField]
    private SessionsManipulator ScenesManipulator;

    [SerializeField]
    private TMPro.TextMeshProUGUI Lable;

    [SerializeField]
    private GameObject PlayIcon;

    [SerializeField]
    private TMPro.TextMeshProUGUI SwapTime;

    // Start is called before the first frame update
    void Start()
    {
        ScenesManipulator.Playing.AddListener(PlayingStateChanged);
        ScenesManipulator.swapTime.AddListener(SwapTimeChanged);
        KineticFieldController.Instance.Session.AddListener(SessionChanged);
    }

    private void SwapTimeChanged(float swTime)
    {
        SwapTime.text = Math.Round(swTime, 2).ToString();
    }

    private void PlayingStateChanged(bool v)
    {
        PlayIcon.SetActive(v);
    }



    private void SessionChanged(KineticSession session)
    {
        if (session!=null && session.ActivePreset.Value!=null)
        {
            session.ActivePreset.AddListener(PresetChanged);
        }
    }

    private void PresetChanged(KineticPreset preset)
    {
        Lable.text = KineticFieldController.Instance.Session.Value.Presets.ToList().IndexOf(preset).ToString();
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
