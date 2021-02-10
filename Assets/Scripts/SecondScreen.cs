using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondScreen : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI DisplaysCount;

    [SerializeField]
    private GameObject Camera;

    [SerializeField]
    private Transform RawImg;

    [SerializeField]
    private Toggle FlipBtn;

    private int lastDisplaysCount = 0;


    private void Start()
    {
        FlipBtn.onValueChanged.AddListener(ToggleFlip);
    }

    private void ToggleFlip(bool v)
    {
        if (v)
        {
            RawImg.localScale = new Vector3(-RawImg.localScale.x, RawImg.localScale.y, RawImg.localScale.z);
        }
        else
        {
            RawImg.localScale = new Vector3(RawImg.localScale.x, RawImg.localScale.y, RawImg.localScale.z);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastDisplaysCount!= Display.displays.Length)
        {
            DisplaysCount.text = Display.displays.Length.ToString();

            if (Display.displays.Length > 1)
            {
                Camera.SetActive(true);
                Display.displays[1].Activate();
            }
            else
            {
                Camera.SetActive(false);
            }

            lastDisplaysCount = Display.displays.Length;

            FlipBtn.gameObject.SetActive(Display.displays.Length > 1);
        }
      
    }
}
