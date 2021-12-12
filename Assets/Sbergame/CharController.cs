using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    private Rigidbody Rb;

    [SerializeField]
    private float Speed = 5f;

    private int force = 1;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Rb.velocity = new Vector3(Input.GetAxis("Horizontal")*Speed, Input.GetAxis("Vertical")*Speed, 0f);
        //Rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0)*Speed*Time.deltaTime);
    }

    public void Increase()
    {
        force++;

        Speed = 25f + force * 3f;


    }
}
