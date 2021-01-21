using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientEditor : MonoBehaviour
{

    public Gradient gradient;

    [ContextMenu("test")]
    public void Test()
    {
        Edit(gradient);
    }

    public void Edit(Gradient gradient)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        this.gradient = gradient;
        GradientPicker.Create(gradient, "Choose color!", SetGradient, GradientFinished);
    }

    private void SetGradient(Gradient currentGradient)
    {
        //gradient = currentGradient;
    }

    private void GradientFinished(Gradient finishedGradient)
    {
        gradient = finishedGradient;
        transform.GetChild(0).gameObject.SetActive(false) ;
        Debug.Log("You chose a Gradient with " + finishedGradient.colorKeys.Length + " Color keys");
    }
}
