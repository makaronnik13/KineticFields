using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject BallBrefab;

    [SerializeField]
    private AnimationCurve WaitingCurve;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBall());
    }

    private IEnumerator SpawnBall()
    {
        while (true)
        {
            if (FindObjectsOfType<EnemyController>().Length< WaitingCurve.Evaluate(Time.timeSinceLevelLoad))
            {
                Spawn();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void Spawn()
    {
        GameObject newBall = Instantiate(BallBrefab);
        newBall.transform.position = new Vector3(UnityEngine.Random.Range(-25f, 25f), UnityEngine.Random.Range(-15f, 15f) , 0f);
    }

}
