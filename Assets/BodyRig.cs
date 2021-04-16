using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class BodyRig : MonoBehaviour
{
    private KinectSensor sensor;
    private BodyFrameReader bodyReader;

    // Start is called before the first frame update
    void Start()
    {
        sensor = KinectSensor.GetDefault();

        if (sensor != null)
        {
            bodyReader = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bodyReader!=null)
        {
            BodyFrame frame = bodyReader. AcquireLatestFrame();

            Debug.Log(frame);

            if (frame != null)
            {
                List<Body> bodies = new List<Body>();

                Debug.Log(frame.BodyCount);

                foreach (Body b in bodies)
                {
                    if (b.IsTracked)
                    {
                        foreach (KeyValuePair<JointType, Windows.Kinect.Joint> joint in b.Joints)
                        {
                            Debug.Log(joint.Key+"/"+joint.Value.Position);
                        }
                    }
                }
            }
        }
    }

    public void OnDisable()
    {
        if (bodyReader != null)
        {
            bodyReader.Dispose();
            bodyReader = null;
        }

        if (sensor != null)
        {
            if (sensor.IsOpen)
            {
                sensor.Close();
            }

            sensor = null;
        }
    }
}
