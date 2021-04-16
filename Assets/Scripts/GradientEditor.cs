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

        GradientColorKey[] keys = gradient.Gradient.colorKeys;
        for (int i = 0; i < keys.Length; i++)
        {
            float mult = Mathf.RoundToInt(gradient.Gradient.Evaluate(keys[i].time).a * 10f);

            float r = keys[i].color.r / mult;
            float g = keys[i].color.g / mult;
            float b = keys[i].color.b / mult;

            keys[i].color = new Color(r, g, b);
        }

        Gradient editingGradient = new Gradient();
        editingGradient.SetKeys(keys, gradient.Gradient.alphaKeys);

        GradientPicker.Create(editingGradient, "Choose color!", SetGradient, GradientFinished);
    }

    private void SetGradient(Gradient currentGradient)
    {
        //gradient = currentGradient;
    }

    private void GradientFinished(Gradient finishedGradient)
    {
        
        GradientColorKey[] keys = finishedGradient.colorKeys;
        
        for (int i = 0; i < keys.Length; i++)
        {
            float mult =  Mathf.RoundToInt(finishedGradient.Evaluate(keys[i].time).a*10f);

            float r = keys[i].color.r * mult;
            float g = keys[i].color.g * mult;
            float b = keys[i].color.b * mult;

            keys[i].color = new Color(r,g,b);
        }

        finishedGradient.SetKeys(keys, finishedGradient.alphaKeys);

        gradient.Gradient = finishedGradient;
        transform.GetChild(0).gameObject.SetActive(false);
        Debug.Log("E1");
        gradient.OnEdited();
    }
}
