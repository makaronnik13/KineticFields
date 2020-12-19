using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondScreen : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI DisplaysCount;

    [SerializeField]
    private GameObject Camera;

    private int lastDisplaysCount = 0;

    // Update is called once per frame
    void Start()
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
        }
      
    }
}
