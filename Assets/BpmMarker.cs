using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmMarker : MonoBehaviour
{
    float time = 0;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {

        time+= Time.deltaTime;
        
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-time*100*BpmManager.Instance.Bpm.Value/120f, 0);

        if (GetComponent<RectTransform>().anchoredPosition.x<=100 && !transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            BpmManager.Instance.Beat();
        }
    }
}
