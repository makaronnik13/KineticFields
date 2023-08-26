using Klak.Ndi;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public ReactiveCommand OnDestroyed { get; private set; } = new ReactiveCommand();

    public bool Controll = true;

    [SerializeField] RenderTexture rt;
    [SerializeField] float _RotationSpeed;
    [SerializeField] float _MoveSpeed;
    [SerializeField] float _ScrollSpeed;
    [SerializeField] private NdiSender sender;

    Camera _cam;

    public Camera Camera
    {
        get
        {
            if (_cam == null)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Observable.NextFrame().Subscribe(_ =>
        {
            if (sender)
            {
                sender.name = gameObject.name;
                if (rt)
                {
                    RenderTexture texture = new RenderTexture(rt);
                    texture.name = gameObject.name;
                    sender.sourceTexture = texture;
                    Camera.targetTexture = texture;
                    sender.ndiName = gameObject.name;
                    sender.enabled = false;

                    Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                    {
                        sender.enabled = true;
                    }).AddTo(this);
                }
            }
        }).AddTo(this);

       
    }

    // Update is called once per frame
    void Update()
    {
        if (!Controll)
        {
            return;
        }
        if (Input.GetMouseButton(1))
        {
            float rotateX = Input.GetAxis("Mouse X");
            float rotateY = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up, -rotateX *Time.deltaTime* _RotationSpeed);
            transform.Rotate(Vector3.right, rotateY * Time.deltaTime * _RotationSpeed);
            //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        float v = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            v = Time.deltaTime * _RotationSpeed * 0.2f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            v = -Time.deltaTime*_RotationSpeed * 0.2f;
        }

        transform.Rotate(Vector3.forward, v);

        float translateForward = Input.GetAxis("Vertical");
        float translateRight = Input.GetAxis("Horizontal");
        float translateUp = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            translateUp = 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            translateUp = -1;
        }

        float scrollDelta = Input.mouseScrollDelta.y;
       // Vector2 camLocalPos = _cam.transform.localPosition;
       // camLocalPos.x = Mathf.Clamp(camLocalPos.x + scrollDelta * _ScrollSpeed, 0, 100);
       // _cam.transform.localPosition = camLocalPos;

        Vector3 forward = Camera.transform.forward;
        Vector3 right = Camera.transform.right;

        Camera.fieldOfView += scrollDelta * -_ScrollSpeed;
        Vector3 translation = (forward * translateForward + right * translateRight + Vector3.up * translateUp)*Time.deltaTime* _MoveSpeed;
        transform.position+= translation;
    }

    private void OnDestroy()
    {
        OnDestroyed.Execute();
    }

    public void ResetNDI()
    {
        sender.enabled = false;
        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            sender.enabled = true;
        }).AddTo(this);
    }
}
