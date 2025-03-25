using System.Linq;
using UnityEngine;
using XNode;

public abstract class ComponentNode<T>: Node
{
    [HideInInspector] public int _componentId;
    [Output] [SerializeField] protected T _component;
    
    public T Component => _component;
    
    public void SetComponent(T component)
    {
        _component = component;
        _componentId = (component as Object).GetInstanceID();

        // Если выходной порт не существует, просто выходим
        if (!Outputs.Any())
        {
            Debug.LogWarning("No outputs found to update.");
            return;
        }

        var outputPort = Outputs.ElementAt(0);

        /*
        // Проверяем, если порт существует и если уже есть соединение
        if (outputPort.Connection != null)
        {
            // Проверяем, если текущее соединение уже соответствует требуемому
            if (outputPort.Connection.node != null && outputPort.Connection.node.GetPort("output") != null)
            {
                Debug.Log("Reconnection not needed. The port is already connected correctly.");
                return; // Если соединение уже есть и оно правильное, ничего не делаем
            }

            Debug.Log("Disconnecting current connection.");
            outputPort.Disconnect(outputPort.Connection); // Отключаем текущее соединение
        }
*/
        
        // Проверяем, что порт подключения и его узел не null
        if (outputPort.Connection?.node != null)
        {
            var connectedPort = outputPort.Connection.node.GetPort("output");
            if (connectedPort != null)
            {
                Debug.Log("Reconnecting to the output port.");
                outputPort?.Connect(connectedPort); // Подключаем заново
                outputPort.Connection?.node.UpdatePorts();
            }
            else
            {
                Debug.LogWarning("Connected port is null.");
            }
        }
        else
        {
            Debug.LogWarning("Output port is null.");
        }
    }
    
    public override object GetValue(NodePort port)
    {
        return _component;
    }
    
}