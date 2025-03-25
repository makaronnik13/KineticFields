using System;
using System.Reflection;
using UniRx;
using UnityEngine;

public abstract class InputSO: ScriptableObject
{
    [HideInInspector]
    public int UniqueId;

    public string OscName;
    public ReactiveProperty<float> Value = new ReactiveProperty<float>();


    public virtual void SetValue(float value)
    {
        Value.Value = value;
    }

    private void OnValidate()
    {
        UniqueId = GetInstanceID();
    }

    public InputSO CreateInstance(InputSO so)
    {
        // Создаем экземпляр нужного типа
        var copy = ScriptableObject.CreateInstance(so.GetType()) as InputSO;
        
        if (copy == null)
        {
            Debug.LogError("Не удалось создать экземпляр типа: " + so.GetType());
            return null;
        }

        // Копируем все поля с помощью рефлексии
        CopyFields(so, copy);

        return copy;
    }

    private void CopyFields(object source, object destination)
    {
        // Получаем все публичные и приватные поля исходного объекта
        var fields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // Копируем значение поля из исходного объекта в целевой объект
            field.SetValue(destination, field.GetValue(source));
        }
    }

}
