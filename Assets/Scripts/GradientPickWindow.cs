using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GradientPickWindow : Singleton<GradientPickWindow>
{
    public GenericFlag<string> selectedGradientId = new GenericFlag<string>("selectedGradientId", "");

    public Action<string> OnPicked = (v)=> { };
    public Action<string> callback;

    [SerializeField]
    private GameObject GradientBtn;

    [SerializeField]
    private GameObject View;

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(PointChanged);
       
        View.SetActive(false);
    }

    void CreateGradientBtns()
    {
        foreach (Transform t in View.transform)
        {
            if (t.gameObject.name!="AddBtn")
            {
                Destroy(t.gameObject);
            }
        }
        int i = 0;
        foreach (GradientInstance gradient in KineticFieldController.Instance.Session.Value.Gradients.Gradients)
        {
            CreateGradientBtn(gradient);
            i++;
        }
    }

    private void PointChanged(KineticPoint obj)
    {
        Hide();
    }

    private void CreateGradientBtn(GradientInstance gradient)
    {
        GameObject newGradientBtn = Instantiate(GradientBtn);
        newGradientBtn.transform.SetParent(View.transform);
        Texture2D newTex = new Texture2D(175, 1, TextureFormat.ARGB32, false);

        for (int i = 0; i < 175; i++)
        {
                newTex.SetPixel(i, 0, gradient.Gradient.Evaluate(i / 175f));
        }

        newTex.Apply();

        newGradientBtn.GetComponent<GradientCurveBtn>().Set(gradient);
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

    public void SelectGradient(string id)
    {
        OnPicked(id);
        if (callback!=null)
        {
            callback.Invoke(id);
        }
        Hide();
    }

    public void Show(string id, Action<string> callback = null)
    {
        selectedGradientId.SetState(id);
        KineticFieldController.Instance.KeysEnabled = false;
        this.callback = callback;
        View.SetActive(true);
        CreateGradientBtns();
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        KineticFieldController.Instance.KeysEnabled = true;
        foreach (Transform t in View.transform)
        {
            if (t.gameObject.name != "AddBtn")
            {
                Destroy(t.gameObject);
            }
        }
        View.SetActive(false);
    }

    public void AddGradient()
    {
        Debug.Log("AddGradient");
        GradientInstance newGradient = new GradientInstance(new Gradient());
        CreateGradientBtn(newGradient);
        selectedGradientId.SetState(newGradient.Id);
        KineticFieldController.Instance.Session.Value.Gradients.Gradients.Add(newGradient);
    }
}
