using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private VisualEffect SpawnVisualEffect;
    [SerializeField]
    private VisualEffect VisualEffect;

    public float Speed = 2f;

    private void Update()
    {
        GetComponent<Rigidbody>().velocity = (FindObjectOfType<CharController>().transform.position - transform.position).normalized * Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharController>())
        {
            Debug.Log("collide");
            other.gameObject.GetComponent<CharController>().Increase();
            DelayDesroy(3);
            SpawnVisualEffect.SendEvent("Spawn");
        }
    }
   

    private void DelayDesroy(int v)
    {
        GetComponent<Collider>().enabled = false;
        VisualEffect.Stop();
        Destroy(gameObject, v);
    }
}
