using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayGridBtn : MonoBehaviour
{
    private bool playing
    {
        get
        {
            return false;
        }
        set
        {
            //BpmManager.Instance.Playing.SetState(value);
        }
    }

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite play, pause;

    public void Toggle()
    {
        playing = !playing;

        if (playing)
        {
            icon.sprite = play;
        }
        else
        {
            icon.sprite = pause;
        }


        }
}
