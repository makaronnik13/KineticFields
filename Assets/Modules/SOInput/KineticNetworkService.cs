using System.Collections;
using System.Collections.Concurrent;
using OscJack;
using UnityEngine;
using Zenject;

public class KineticNetworkService : MonoBehaviour
{
    [SerializeField] private int port = 3000;

    private KineticInputService _kineticInputService;
    private OscServer _server;
    
    private readonly ConcurrentQueue<(string, float)> _messages = new ConcurrentQueue<(string, float)>();

    [Inject]
    void Construct(KineticInputService kineticInputService)
    {
        _kineticInputService = kineticInputService;
    }
    
    private void Start()
    {
        StartCoroutine(StartListen());
    }

    private IEnumerator StartListen()
    {
        _server = new OscServer(port);
        
        _server.MessageDispatcher.AddCallback(string.Empty, 
            (string address, OscDataHandle data) =>
            {
                float v = data.GetElementAsFloat(0);
                _messages.Enqueue((address, v)); // Добавляем в очередь
            }
        );

        yield return null; // Небольшая задержка перед запуском
    }

    private void Update()
    {
        while (_messages.TryDequeue(out var message))
        {
            string address = message.Item1;
            float v = message.Item2;

            InputSO input = _kineticInputService.GetItemInstance(address);

            Debug.Log(address + " " + input);
            
            if (input != null)
            {
                if (input is KnobSO)
                {
                    input.Value.Value = v;   
                }

                if (input is ToggleSO)
                {
                    if (input.Value.Value == 0)
                    {
                        Debug.Log("turn on");
                        input.Value.Value = 1;
                    }
                    else
                    {
                        Debug.Log("turn off");
                        input.Value.Value = 0;
                    }
                }

                if (input is ButtonSO)
                {
                    (input as ButtonSO).Activate();
                }
                
            }
        }
    }

    private void OnDestroy()
    {
        _server?.Dispose(); // Закрываем сервер при завершении
    }
}