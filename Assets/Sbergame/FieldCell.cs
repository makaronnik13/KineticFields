using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldCell : MonoBehaviour
{
    public AnimationCurve ExplosionCurve;
    private Image img;

    public float BorderWidth = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        img.material = new Material(img.material);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("test")]
    public void Test()
    {
        Explode(2f);
    }

    public void Explode(float v)
    {
        StartCoroutine(ExplodeCoroutine(v));
    }

    private IEnumerator ExplodeCoroutine(float v)
    {
        float t = v;
        while (t>0)
        {
            t -= Time.deltaTime;
            img.material.SetFloat("_BorderWidth", 1.2f-ExplosionCurve.Evaluate((1-t/v)));
            yield return null;
        }
    }

    public void MakeDamage()
    {
        Debug.Log("Damage");
    }
}
