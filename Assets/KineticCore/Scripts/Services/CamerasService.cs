using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class CamerasService : MonoBehaviour
{
    [SerializeField]
    private CameraController cameraPrefab;
    [SerializeField]
    private CameraController mainCamera;

    public ReactiveProperty<CameraController> ActiveCamera { get; private set; } = new ReactiveProperty<CameraController>();

    private PrefabCreator prefabCreator;
    private List<CameraController> cameras = new List<CameraController>();

    [Inject]
    public void Construct(PrefabCreator prefabCreator)
    {
        this.prefabCreator = prefabCreator;
        ActiveCamera.Subscribe(v =>
        {
            mainCamera.Controll = v == null;
            foreach (CameraController cc in cameras)
            {
                cc.ResetNDI();
                cc.Controll = cc==v;
            }
        }).AddTo(this);
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (ActiveCamera.Value)
            {
                mainCamera.transform.position = ActiveCamera.Value.transform.position;
                mainCamera.transform.rotation = ActiveCamera.Value.transform.rotation;
                mainCamera.Camera.fieldOfView = ActiveCamera.Value.Camera.fieldOfView;
            }
        }).AddTo(this);
    }

    public CameraController CreateCamera()
    {
        CameraController cameraController = prefabCreator.Create<CameraController>(cameraPrefab, transform);
        cameras.Add(cameraController);
        cameraController.gameObject.name = "VirtualCamera_"+cameras.Count.ToString();
        cameraController.OnDestroyed.Subscribe(_ =>
        {
            cameras.Remove(cameraController);
        }).AddTo(this);
        ActiveCamera.Value = cameraController;
        return cameraController;
    }

    public void SelectCamera(CameraController cameraController)
    {
        ActiveCamera.Value = cameraController;
    }

}
