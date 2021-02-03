using System;
using UnityEngine;
using UnityEngine.UI;

public class GradientPick: MonoBehaviour
{
    public Action<string> OnGradientPicked = (v) => { };

    private string currentId;

    [SerializeField]
    private Image Img;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            FindObjectOfType<GradientPickWindow>().Show(currentId, (v) =>
            {
                OnGradientPicked(v);
                SetValue(SessionsManipulator.Instance.Gradients.GetGradient(v));
            });
        });
    }

    public void SetValue(GradientInstance gr)
    {
        currentId = gr.Id;
        Img.sprite = FindObjectOfType<GradientPickWindow>().GetImage(gr.Gradient);
    }
}