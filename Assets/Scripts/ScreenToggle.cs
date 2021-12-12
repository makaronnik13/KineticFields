using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenToggle : MonoBehaviour
{
    [SerializeField]
    private Toggle Toggle;

    public GameObject Camera;

    private int screenId;

    public void Init(GameObject go, int screenId)
    {
        Camera = go;
        Toggle.SetIsOnWithoutNotify(false);
        Toggle.onValueChanged.AddListener(ToggleChanged);
        Toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Screen " + (screenId+1);
    }

    private void ToggleChanged(bool v)
    {
        Camera.SetActive(v);
        if (v)
        {
            Display.displays[screenId+1].Activate();
            Camera.GetComponent<Camera>().targetDisplay = screenId+1;
        }
    }
}
