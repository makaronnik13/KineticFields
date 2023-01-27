using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SoundReactiveScale : MonoBehaviour
{

    [SerializeField] private BaseSignalSource signalSource;
    [SerializeField] private Vector3 axisMultiplyers;
    [SerializeField] private float speed = 1f;
    private Vector3 scale;

    private void Start()
    {
        scale = transform.localScale;
        signalSource.Signal.Subscribe(v =>
        {
            Vector3 newVec = new Vector3(scale.x * axisMultiplyers.x, scale.y * axisMultiplyers.y, scale.z * axisMultiplyers.z) * v;
            transform.localScale = Vector3.Lerp(transform.localScale, newVec, speed) ;
        }).AddTo(this);
    }
}
