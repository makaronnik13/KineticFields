using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PongBall : MonoBehaviour
{
    [SerializeField]
    private float StartSpeed;

    [SerializeField]
    private float SpeedUp = 0.1f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = (Vector3.right * StartSpeed);
    }

    public void Crash()
    {
        GetComponent<Collider>().enabled = false;
        rb.velocity = Vector3.zero;
        Destroy(gameObject, 3f);
        GetComponentInChildren<VisualEffect>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity += rb.velocity.normalized * Time.deltaTime * SpeedUp;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0).normalized*rb.velocity.magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PongPlayer>()!=null)
        {
            Debug.Log("add speed");
            rb.velocity += (transform.position - collision.transform.position).normalized * SpeedUp*2f;
        }
        
    }
}
