using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondScreen : Singleton<SecondScreen>
{
    [SerializeField]
    private GameObject Camera;

    [SerializeField]
    private Transform RawImg;

    [SerializeField]
    private Toggle FlipBtn;

    [SerializeField]
    private GameObject CameraPrefab;

    public List<GameObject> Cameras = new List<GameObject>();

    private GenericFlag<int> screensCount = new GenericFlag<int>("screens", 1);

    private void Start()
    {
        FlipBtn.onValueChanged.AddListener(ToggleFlip);
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.fullScreen = true;
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
        while (screensCount.Value < Display.displays.Length)
        {
           // Display.displays[screensCount.Value].Activate();
            screensCount.SetState(screensCount.Value+1);
            GameObject newCamera = Instantiate(CameraPrefab);
            newCamera.transform.SetParent(transform.GetChild(0));
            newCamera.transform.localPosition = Vector3.zero;
            newCamera.transform.localScale = Vector3.one;
            //newCamera.GetComponent<Camera>().targetDisplay = screensCount.Value;
            newCamera.gameObject.SetActive(false);
            Cameras.Add(newCamera);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(transform.forward, 90f);
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = true;
        }
    }
}
