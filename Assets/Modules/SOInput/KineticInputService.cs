using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class KineticInputService : MonoBehaviour
{
    public List<InputSO> InputInstances { get; private set; } = new List<InputSO>();
    public ReactiveCommand OnItemsLoaded = new ReactiveCommand();

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        InputInstances.Clear();

        var inputSOs = Resources.LoadAll<InputSO>(""); // ��������� InputSO
        foreach (var so in inputSOs)
        {
            var instance = so.CreateInstance(so);
            InputInstances.Add(instance);
        }

        OnItemsLoaded.Execute();
    }

    public InputSO GetItemInstance(InputSO so)
    {
        return InputInstances.FirstOrDefault(i => i.UniqueId == so.UniqueId);
    }
}
