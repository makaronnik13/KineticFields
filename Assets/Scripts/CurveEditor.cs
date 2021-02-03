using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveEditor : MonoBehaviour
{
    public float HeigthMult = 1f;

    public static CurveEditor Instance;

    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private LineRenderer Line;

    [SerializeField]
    private GameObject pointPrefab;

    private List<GameObject> points = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public Sprite MakeScreenshot(AnimationCurve curve)
    {
        View.SetActive(true);
        SetCurve(curve);
        RenderTexture rt = new RenderTexture(1024, 512, 24);
        Camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(1024, 512, TextureFormat.RGB24, false);
        Camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 1024, 512), 0, 0);
        screenShot.Apply();
        Camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        View.SetActive(false);
        Line.positionCount = 0;
        return Sprite.Create(screenShot, new Rect(0,0,screenShot.width, screenShot.height), Vector3.one/2f);
    }

    public void SetCurve(AnimationCurve curve)
    {
        Line.positionCount = 100;

        for (int i = 0; i < 100; i++)
        {

            Line.SetPosition(i, new Vector3(i/30f, curve.Evaluate(i/100f)*HeigthMult, 0));   
        }
    }
}
