using System;
using UnityEngine;
using UnityEngine.UI;

public class GradientPick: MonoBehaviour
{
    public Action<int> OnGradientPicked = (v) => { };

    private int currentId;

    [SerializeField]
    private Image Img;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            FindObjectOfType<GradientPickWindow>().Show(currentId, (v) =>
            {
                OnGradientPicked(v);
                SetValue(v);
            });
        });
    }

    public void SetValue(int gradientId)
    {
        currentId = gradientId;
        Img.sprite = FindObjectOfType<GradientPickWindow>().GetImage(DefaultResources.Settings.Gradients[gradientId]);
    }
}