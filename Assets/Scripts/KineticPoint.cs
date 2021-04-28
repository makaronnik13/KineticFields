using System;
using System.Collections;
using System.Linq;
using com.armatur.common.flags;
using UnityEngine;
using UnityEngine.EventSystems;

public class KineticPoint : MonoBehaviour
{
    private float StartForce = 5;
    private float GravityMultiplyer
    {
        get
        {
            return PointsController2D.Instance.gravityMultiplyer;
        }
    }
    private Rigidbody2D rigidbody;
    public Rigidbody2D RigidBody
    {
        get
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }
            return rigidbody;
        }
    }
    private bool dragging = false;
    private Coroutine doubleClickCoroutine;

    [SerializeField]
    private SpriteRenderer Icon, Border;

    [SerializeField]
    private GameObject Selector;

    private KineticPointInstance point = null;
    public KineticPointInstance Point
    {
        get
        {
            if (PresetsLerper.Instance.View.activeInHierarchy)
            {
                if (KineticFieldController.Instance.Session.Value!=null && point!=null)
                {
                    point = KineticFieldController.Instance.Session.Value.AveragePreset.Points.FirstOrDefault(p => p.Id == point.Id);
                }
                else
                {
                    return null;
                }
            }
            
            return point;
        }
        set
        {
            point = value;
        }
    }

    private void Awake()
    {
        Selector.SetActive(false);
    }

    void Start()
    {
        KineticFieldController.Instance.ActivePoint.AddListener(ActivePointChanged);
        KineticFieldController.Instance.SelectedSource.AddListener(SourceChanged);
    }

    public void Init(KineticPointInstance point)
    {
 
        if (Point!=null)
        {
            Point.Radius.Value.RemoveListener(RadiusChanged);
            Point.Volume.Value.RemoveListener(VolumeChanged);
            Point.Active.RemoveListener(ActiveChanged);
        }

        Point = point;

        Point.Radius.Value.AddListener(RadiusChanged);
        Point.Volume.Value.AddListener(VolumeChanged);
        Point.Active.AddListener(ActiveChanged);
        if (point.Position!=Vector3.zero)
        {
            transform.position = point.Position;
        }

        Point.Radius.Value.RaiseEvent();
        Point.Volume.Value.RaiseEvent();
        Point.Deep.Value.RaiseEvent();

    }

    private void ActiveChanged(bool v)
    {
        GetComponent<CircleCollider2D>().isTrigger = !v;
        if (v)
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 1f);
            Border.color = new Color(Border.color.r, Border.color.g, Border.color.b, 1f);
            RadiusChanged(Point.Radius.Value.Value);
            VolumeChanged(point.Radius.Value.Value);

        }
        else
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0.1f);
            Border.color = new Color(Border.color.r, Border.color.g, Border.color.b, 0.1f);
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            RadiusChanged(0);
            VolumeChanged(0);
        }
    }
    private void RadiusChanged(float v)
    {
        if (Point.Active.Value)
        {
            //KineticFieldController.Instance.Visual.SetFloat("P" + Point.Id + "Radius", v);
        }
        else
        {
            //KineticFieldController.Instance.Visual.SetFloat("P" + Point.Id + "Radius", 0);
        }
    }
    private void VolumeChanged(float v)
    {
        if (Point.Active.Value)
        {
            //KineticFieldController.Instance.Visual.SetFloat("P" + Point.Id + "Value", v);
        }
        else
        {
            //KineticFieldController.Instance.Visual.SetFloat("P" + Point.Id + "Value", 0);
        }
    }
    private void ActivePointChanged(KineticPoint p)
    {
        GetComponent<Collider2D>().enabled = p != this;
    }

    private void SourceChanged(ISource source)
    {
        if (point == null)
        {
            return;
        }
        if (source == null)
        {
            Selector.SetActive(false);
            return;
        }

        Selector.SetActive(point.Deep.Source == source || point.Radius.Source == source || point.Volume.Source == source);

        if (point == KineticFieldController.Instance.Session.Value.ActivePreset.Value.MainPoint)
        {
            Selector.SetActive(Selector.activeInHierarchy 
                || KineticFieldController.Instance.Session.Value.ActivePreset.Value.FarCutPlane.Source == source
                || KineticFieldController.Instance.Session.Value.ActivePreset.Value.NearCutPlane.Source == source
                || KineticFieldController.Instance.Session.Value.ActivePreset.Value.ParticlesCount.Source == source
                || KineticFieldController.Instance.Session.Value.ActivePreset.Value.Lifetime.Source == source);
        }
    }

    // Update is called once per frame
    void Update()
    {
  
        /*
        if (Point==null)
        {
            return;
        }

        if (Point.Active.Value && !dragging)
        {
            if (RigidBody.velocity.magnitude < 0.01f)
            {
                RigidBody.velocity = new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
            }

            Vector3 velocity = RigidBody.velocity.normalized * Point.Speed.Value.Value * PointsController2D.Instance.globalSpeedMultiplyer;

            RigidBody.velocity = velocity;

            RigidBody.mass = Point.Radius.Value.Value;
            foreach (KineticPoint kp in FindObjectsOfType<KineticPoint>())
            {
                if (kp != this && kp.Point.Active.Value)
                {
                    Vector2 dir = transform.position - kp.transform.position;

                    kp.RigidBody.AddForce(2 * GravityMultiplyer * dir * kp.Point.Radius.Value.Value * Point.Radius.Value.Value / Mathf.Pow(dir.magnitude, 2) * Time.deltaTime);
                }
            }
        }


        transform.localScale = (0.2f + Point.Deep.Value.Value*0.05f)*Vector3.one * (Camera.main.fieldOfView/35f);

        //point.Position = transform.position;
        */
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (doubleClickCoroutine!=null)
        {
            Point.Active.SetState(!point.Active.Value);
            StopCoroutine(doubleClickCoroutine);
            doubleClickCoroutine = null;
        }
        else
        {
            doubleClickCoroutine = StartCoroutine(DoubleClick());
        }

        KineticFieldController.Instance.ActivePoint.SetState(this);
        dragging = true;
    }

    private IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(0.3f);
        doubleClickCoroutine = null;
    }

    private void OnMouseDrag()
    {
        /*
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        curPosition.z = transform.position.z;
        transform.position = curPosition;

        Point.Position = transform.position;
        */
    }

    private void OnMouseUp()
    {
        /*
        dragging = false;
        if (Point.Active.Value)
        {
            RigidBody.velocity = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }*/
    }



}
