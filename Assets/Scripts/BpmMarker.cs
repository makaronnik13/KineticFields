using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmMarker : MonoBehaviour
{
    public float time = 0;

    // Update is called once per frame
    void Update()
    {

        time+= Time.deltaTime;
        


        GetComponent<RectTransform>().anchoredPosition = new Vector2(-time*100*BpmManager.Instance.Bpm.Value/120f, 0);

        if (GetComponent<RectTransform>().anchoredPosition.x<=-100 && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            BpmManager.Instance.Beat();
        }

    }


    public void Reset()
    {
        time = 0f;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
