using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaeraController : MonoBehaviour
{
    [SerializeField]
    private float FowSensivity;
    [SerializeField]
    private float MoveSensivity;
    private void Update()
    {
        if (!PresetsLerper.Instance.Lerping.Value)
        {
            GetComponent<Camera>().fieldOfView += Input.GetAxis("Deep") * Time.deltaTime * FowSensivity;
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * Time.deltaTime * MoveSensivity);
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * MoveSensivity);
        }

    }

}
