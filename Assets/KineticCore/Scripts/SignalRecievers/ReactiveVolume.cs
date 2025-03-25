using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class ReactiveVolume : MonoBehaviour
{
    [Serializable]
    public class SignalVolumePair
    {
        [HideInInspector]
        public string signalSourceName; // Для отладки, чтобы показывать имя источника

        [Required, Tooltip("Объект, реализующий ISignalSource")]
        [SerializeReference]
        public BaseSignalSource signalSource; // Ссылка на источник сигнала

        [ValueDropdown(nameof(GetFieldNames)), Tooltip("Имя числового поля в формате ComponentName/FieldName")]
        public string fieldName; // Имя числового поля в формате ComponentName/FieldName

        public Volume volume;

        // Список доступных полей
        private IEnumerable<string> GetFieldNames()
        {
            if (volume == null || volume.sharedProfile == null)
            {
                return new List<string> { "Volume или VolumeProfile не назначены" };
            }

            var profile = volume.sharedProfile;

            return profile.components
                          .SelectMany(component =>
                              component.GetType().GetFields()
                                       .Where(field => typeof(VolumeParameter<float>).IsAssignableFrom(field.FieldType))
                                       .Select(field => $"{component.GetType().Name}/{field.Name}")); // Формат ComponentName/FieldName
        }

        
    }

    [Title("Signal Volume Pairs")]
    [ListDrawerSettings(Expanded = true)]
    [SerializeField]
    private List<SignalVolumePair> signalVolumePairs = new List<SignalVolumePair>();

    void Start()
    {

        foreach (var pair in signalVolumePairs)
        {
            if (pair.signalSource == null || string.IsNullOrEmpty(pair.fieldName))
            {
                Debug.LogWarning($"Invalid SignalVolumePair configuration: {pair}");
                continue;
            }

            // Разделяем имя на ComponentName и FieldName
            var split = pair.fieldName.Split('/');
            if (split.Length != 2)
            {
                Debug.LogWarning($"Invalid field format: {pair.fieldName}");
                continue;
            }

            var componentName = split[0];
            var fieldName = split[1];

            // Получаем VolumeComponent по имени
            var component = pair.volume.sharedProfile.components.FirstOrDefault(c => c.GetType().Name == componentName);
            if (component == null)
            {
                Debug.LogWarning($"Component {componentName} not found in VolumeProfile.");
                continue;
            }

            // Получаем поле внутри компонента
            var field = component.GetType().GetField(fieldName);
            if (field == null || !typeof(VolumeParameter<float>).IsAssignableFrom(field.FieldType))
            {
                Debug.LogWarning($"Field {fieldName} not found or is not a float in {componentName}.");
                continue;
            }

            // Подписываемся на сигнал и обновляем значение
            var volumeField = field.GetValue(component) as VolumeParameter<float>;
            if (volumeField == null)
            {
                Debug.LogWarning($"Failed to cast field {fieldName} in {componentName} to VolumeParameter<float>.");
                continue;
            }

            pair.signalSource.MultipliedSignal.Subscribe(signalValue =>
            {
                volumeField.value = signalValue;
            }).AddTo(this);

            pair.signalSourceName = pair.signalSource.GetType().Name;
        }
    }
}