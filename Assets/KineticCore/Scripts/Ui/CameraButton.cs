using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CameraButton : MonoBehaviour
{
    [SerializeField]
    private GameObject frame;

    [Inject]
    public void Construct([InjectOptional]CameraController cameraController, CamerasService camerasService)
    {
        if (cameraController == null)
        {
            return;
        }

        GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
        {
            if (camerasService.ActiveCamera.Value == cameraController)
            {
                camerasService.SelectCamera(null);
            }
            else
            {
                camerasService.SelectCamera(cameraController);
            }
           
        }).AddTo(this);

        camerasService.ActiveCamera.Subscribe(cam =>
        {
            Debug.Log(cam);
            if (cam)
            {
                Debug.Log(cam.gameObject.name);
            }
            frame.gameObject.SetActive(cam == cameraController);
        }).AddTo(this);

        cameraController.OnDestroyed.Subscribe(_ =>
        {
            Destroy(gameObject);
        }).AddTo(this); 
    }
}
