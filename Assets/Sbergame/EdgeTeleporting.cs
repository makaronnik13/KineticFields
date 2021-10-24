using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTeleporting : MonoBehaviour
{
    [SerializeField]
    private Vector2 FieldSize = new Vector2(100f,20f);

    [SerializeField]
    private Vector2 EdgeSize = new Vector2(10f, 10f);

    [SerializeField]
    private GameObject View;

    private GameObject Copy;

    private void Update()
    {
        if (transform.position.x < -FieldSize.x/2f)
        {
            transform.position = new Vector3(transform.position.x+FieldSize.x, transform.position.y, transform.position.z);
        }

        if (transform.position.x > FieldSize.x/2f)
        {
            transform.position = new Vector3(transform.position.x-FieldSize.x, transform.position.y, transform.position.z);
        }

        if (transform.position.y < -FieldSize.y/2f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y+FieldSize.y, transform.position.z);
        }

        if (transform.position.y > FieldSize.y/2f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - FieldSize.y, transform.position.z);
        }

        if ((transform.position.x < -FieldSize.x / 2f + EdgeSize.x) ||
            (transform.position.x > FieldSize.x / 2f - EdgeSize.x) ||
            (transform.position.y < -FieldSize.y / 2f + EdgeSize.y) ||
            (transform.position.y > FieldSize.y / 2f - EdgeSize.y))
        {
            if (Copy == null)
            {
                Copy = Instantiate(View);
            }

        }
        else
        {
            if (Copy!=null)
            {
                Destroy(Copy);
            }
        }


        if (Copy != null)
        {
            
            if (transform.position.x<0)
            {
                Copy.transform.position = new Vector3(transform.position.x+FieldSize.x, transform.position.y, transform.position.z);
            }
            else
            {
                Copy.transform.position = new Vector3(transform.position.x - FieldSize.x, transform.position.y, transform.position.z);
            }

            
        }
    }
}
