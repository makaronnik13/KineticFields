using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LerperBackground : MonoBehaviour, IPointerClickHandler
{
    private Coroutine doubleClickCoroutine = null;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");

        if (doubleClickCoroutine == null)
        {
            doubleClickCoroutine = StartCoroutine(DoubleClick());
        }
        else
        {
            PresetsLerper.Instance.CreateNewPreset();
        }
    }

    private IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(0.3f);
        doubleClickCoroutine = null;
    }
}
