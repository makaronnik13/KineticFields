using System.Collections.Generic;
using Zenject;
using UnityEngine;

public class PrefabCreator
{
    private readonly DiContainer diContainer;

    public PrefabCreator(DiContainer diContainer)
    {
        this.diContainer = diContainer;
    }

    public GameObject Create(Object prefab, Transform parent)
    {
        GameObject newGo = diContainer.InstantiatePrefab(prefab, Vector3.zero, Quaternion.identity, parent);
        newGo.transform.localScale = Vector3.one;
        newGo.transform.localPosition = Vector3.zero;
        newGo.SetActive(true);
        return newGo;
    }

    public T Create<T>(Object prefab, Transform parent, bool setPositionAndScale = true)
    {
        T newComponent = diContainer.InstantiatePrefabForComponent<T>(prefab, Vector3.zero, Quaternion.identity, parent);
        if (setPositionAndScale)
        {
            (newComponent as MonoBehaviour).transform.localScale = Vector3.one;
            (newComponent as MonoBehaviour).transform.localPosition = Vector3.zero;
        }
        (newComponent as MonoBehaviour).gameObject.SetActive(true);
        return newComponent;
    }

    public T Create<T>(Object prefab, Transform parent, IEnumerable<object> args, bool setPositionAndScale = true)
    {
        T newComponent = diContainer.InstantiatePrefabForComponent<T>(prefab, Vector3.zero, Quaternion.identity, parent, args);
        if (setPositionAndScale)
        {
            (newComponent as MonoBehaviour).transform.localScale = Vector3.one;
            (newComponent as MonoBehaviour).transform.localPosition = Vector3.zero;
        }
        (newComponent as MonoBehaviour).gameObject.SetActive(true);
        return newComponent;
    }

    public GameObject CreateEmpty(string name)
    {
        GameObject newGo = new GameObject(name);
        newGo.transform.localScale = Vector3.one;
        newGo.SetActive(true);
        return newGo;
    }
}