using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientEditor : Singleton<GradientEditor>
{

    public GradientInstance gradient;


    public void Edit(GradientInstance gradient)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        this.gradient = gradient;
        GradientPicker.Create(gradient.Gradient, "Choose color!", SetGradient, GradientFinished);
    }

    private void SetGradient(Gradient currentGradient)
    {
        //gradient = currentGradient;
    }

    private void GradientFinished(Gradient finishedGradient)
    {
        gradient.Gradient = finishedGradient;
        transform.GetChild(0).gameObject.SetActive(false);
        gradient.OnEdited();
        SessionsManipulator.Instance.SaveGradients();
    }
}
