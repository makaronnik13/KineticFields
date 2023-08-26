using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CamerasPanel : MonoBehaviour
{
    [SerializeField]
    private Button addCameraButton, removeCameraButton;

    [SerializeField]
    private CameraButton cameraButtonPrefab;

    [SerializeField]
    private GameObject cameraInfo;

    [SerializeField]
    private TMP_Text cameraLable;

    private PrefabCreator prefabCreator;
    private CamerasService camerasService;

    [Inject]
    public void Construct(PrefabCreator prefabCreator, CamerasService camerasService)
    {
        this.camerasService = camerasService;
        this.prefabCreator = prefabCreator;

        addCameraButton.OnClickAsObservable().Subscribe(_ =>
        {
            CameraController cc = camerasService.CreateCamera();
            prefabCreator.Create<CameraButton>(cameraButtonPrefab, transform, new List<object>() { cc});
        }).AddTo(this);

        removeCameraButton.OnClickAsObservable().Subscribe(_ =>
        {
            Destroy(camerasService.ActiveCamera.Value.gameObject);
        }).AddTo(this);

        camerasService.ActiveCamera.Subscribe(cam =>
        {
            cameraInfo.SetActive(cam!=null);
            if (cam)
            {
                cameraLable.text = cam.name;
            }
        }).AddTo(this);
    }
}
