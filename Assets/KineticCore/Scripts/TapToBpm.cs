using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class TapToBpm : MonoBehaviour
{
    [SerializeField] private ButtonSO TapInput;
    [SerializeField] private ButtonSO ResyncInput;
    
    [SerializeField]
    private ConstantBPMSource bpmSource;

    private float lastTapTime = -1f;  // Время последнего тапа
    private float currentBpm;  // Текущее значение BPM
    private const float MINIMAL_BPM = 20f;  // Минимальное BPM
    private Subject<float> tapTimes = new Subject<float>();  // Поток временных интервалов между тапами

    [Inject]
    void Construct(KineticInputService inputService)
    {
        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            (inputService.GetItemInstance(TapInput) as ButtonSO).OnPressed += Tap;
            (inputService.GetItemInstance(ResyncInput) as ButtonSO).OnPressed += Resync;
        }).AddTo(this);
        
        // Подписка на поток tapTimes для вычисления BPM
        tapTimes
            .ThrottleFirst(System.TimeSpan.FromSeconds(1f / MINIMAL_BPM))  // Сброс интервала через заданное время
            .Subscribe(interval =>
            {
                // Если второй тап был совершён достаточно быстро
                if (lastTapTime != -1f)
                {
                    // Вычисляем BPM
                    currentBpm = 60f / interval; // 60 секунд / интервал между тапами
                    bpmSource.Bpm.Value = Mathf.RoundToInt(currentBpm); // Обновляем BPM источника
                    Debug.Log($"New BPM: {currentBpm}");
                }
                
                lastTapTime = Time.time;  // Обновляем время последнего тапа
            });
    }

    private void Resync()
    {
        bpmSource.Restart();
    }

    
    
    private void Tap()
    {
// Если это первый тап или прошло слишком много времени
        if (lastTapTime == -1f || Time.time - lastTapTime > 1f / MINIMAL_BPM)
        {
            // Сбрасываем таймер, так как второй тап был слишком поздно
            lastTapTime = Time.time;
            currentBpm = 0f;
        }
        else
        {
            // Отправляем интервал между тапами в поток
            tapTimes.OnNext(Time.time - lastTapTime);
        }
    }

    void OnDestroy()
    {
        ResyncInput.OnPressed -= Resync;
        TapInput.OnPressed += Tap;
        tapTimes.Dispose();  
    }
}
