using UnityEngine;
using Zenject;
using UniRx;

public class SyncControllView<T> : MonoBehaviour
{
    [SerializeField]
    private InputSO InputScriptableObject;

    protected InputSO InputInstance;

    [Inject]
    public void Construct(KineticInputService inputService)
    {
        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            InputInstance = inputService.GetItemInstance(InputScriptableObject);
        }).AddTo(this);
    }
}
