using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GradientPickWindow : MonoBehaviour
{
    public Action<int> OnPicked = (v)=> { };
    public Action<int> callback;

    [SerializeField]
    private GameObject GradientBtn;

    [SerializeField]
    private GameObject View;

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(PointChanged);
        int i = 0;
        foreach (Gradient gradient in DefaultResources.Settings.Gradients)
        {
            CreateGradientBtn(gradient, i);
            i++;
        }
        View.SetActive(false);
    }

    private void PointChanged(KineticPoint obj)
    {
        Hide();
    }

    private void CreateGradientBtn(Gradient gradient, int id)
    {
        GameObject newGradientBtn = Instantiate(GradientBtn);
        newGradientBtn.transform.SetParent(View.transform);
        Texture2D newTex = new Texture2D(175, 1, TextureFormat.ARGB32, false);

        for (int i = 0; i < 175; i++)
        {
                newTex.SetPixel(i, 0, gradient.Evaluate(i / 175f));
        }

        newTex.Apply();

        newGradientBtn.GetComponent<Image>().sprite = GetImage(gradient);
        newGradientBtn.GetComponent<Button>().onClick.AddListener(()=>SelectGradient(id));
    }

    public Sprite GetImage(Gradient gradient)
    {
        Texture2D newTex = new Texture2D(175, 1, TextureFormat.ARGB32, false);

        for (int i = 0; i < 175; i++)
        {
            newTex.SetPixel(i, 0, gradient.Evaluate(i / 175f));
        }

        newTex.Apply();

        return Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector3.one / 2f);
    }

    private void SelectGradient(int id)
    {
        OnPicked(id);
        if (callback!=null)
        {
            callback.Invoke(id);
        }
        Hide();
    }

    [ContextMenu("Show")]
    public void Test()
    {
        Show(3);
    }

    public void Show(int id, Action<int> callback = null)
    {
        this.callback = callback;
        View.SetActive(true);
        foreach (Transform t in View.transform)
        {
            t.GetChild(0).gameObject.SetActive(false);
        }
        View.transform.GetChild(id).GetChild(0).gameObject.SetActive(true);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        View.SetActive(false);
    }
}
