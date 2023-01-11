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
        
        
        if (GetComponent<RectTransform>().anchoredPosition.x<=-100 && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }


    public void Reset()
    {
        time = 0f;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
