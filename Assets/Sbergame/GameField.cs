using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameField : MonoBehaviour
{

    [SerializeField]
    private float Period = 1f;

    private List<FieldCell> cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = GetComponentsInChildren<FieldCell>().ToList();
        StartCoroutine(ExplodeCell());
    }

    private IEnumerator ExplodeCell()
    {
        while (true)
        {
            cells.OrderBy(c => Guid.NewGuid()).FirstOrDefault().Explode(UnityEngine.Random.Range(4f, 6f));

            yield return new WaitForSeconds(Period);
        }
    }

  
}
