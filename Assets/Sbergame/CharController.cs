using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rb;

    [SerializeField]
    private float Speed = 5f;

    // Update is called once per frame
    void Update()
    {
        Rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0)*Speed*Time.deltaTime);
    }
}
