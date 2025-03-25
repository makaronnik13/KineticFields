using Assets.WasapiAudio.Scripts.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class CloudSourceBtn : MonoBehaviour
{
    [SerializeField]
    private VisualEffect Effect;

    [SerializeField]
    private VisualEffectAsset KinnectAsset, MeshAsset;

    [SerializeField]
    private bool fromMesh = false;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite kinect, mesh;

    public void Toggle()
    {
        fromMesh = !fromMesh;

        if (fromMesh)
        {
            icon.sprite = mesh;
            Effect.visualEffectAsset = MeshAsset;
        }
        else
        {
            icon.sprite = kinect;
            Effect.visualEffectAsset = KinnectAsset;
        }

    }
}
