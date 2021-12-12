using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongPlayer : MonoBehaviour
{
    public bool firstPlayer;

    public float Speed;
    void Update()
    {
        if (firstPlayer)
        {
            transform.Translate(Vector3.up * Input.GetAxis("Horizontal")*Speed*Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * Speed * Time.deltaTime);
        }
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -17f, 17f), transform.position.z);
    }
}
