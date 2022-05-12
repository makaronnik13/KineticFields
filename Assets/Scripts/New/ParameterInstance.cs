using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Linq;

namespace KineticFields
{
    public class ParameterInstance: ITickable
    {
        private ParameterData data;
        private FFTService fftService;

        public ReactiveProperty<float> Value { get; private set; } = new ReactiveProperty<float>();
        public ReactiveProperty<bool> UseEnvelope => data.UseEnvelope;
        public ReactiveProperty<bool> UseFft => data.UseFft;

        public ReactiveProperty<SourceType> SourceType => data.SourceType;

        public string Name => data.Name;

        public float MaxValue => data.Max;
        public float MinValue => data.Min;

        public float UserValue
        {
            get
            {
                return data.UserValue;
            }
            set
            {
                data.UserValue = value;
            }
        }

        public float UserMaxValue
        {
            get
            {
                return data.UserMax;
            }
            set
            {
                data.UserMax = value;
            }
        }

        public float UserMinValue 
        {
            get
            {
                return data.UserMin;
            }
            set
            {
                data.UserMin = value;
            }
        }

        [Inject]
        public void Construct(ParameterData data, FFTService fftService)
        {
            this.fftService = fftService;
            this.data = data;
            SetValue();
        }

        private void SetValue()
        {
            float min = data.Min;
            float max = data.Max;
            float size = max - min;

            if (data.SourceType.Value != KineticFields.SourceType.None)
            {
                max = min + data.MaxGap * size;
                min = min + data.MinGap * size;
                size = max - min;
            }



            float sourceValue = 0;
            switch (data.SourceType.Value)
            {
                case KineticFields.SourceType.None:
                    break;
                case KineticFields.SourceType.FFT:
                    sourceValue = fftService.OutputSpectrum.Value.Average();
                    break;
                case KineticFields.SourceType.Body:
                    break;
            }

            sourceValue *= data.Multiplyer;

            if (data.UseCurve)
            {
                data.Value.Value = data.Curve.Evaluate(sourceValue);
            }
            else
            {
                data.Value.Value = data.Value.Value;
            }

        }

        public void Tick()
        {
            SetValue();
        }

        public class Factory: PlaceholderFactory<ParameterData, ParameterInstance>
        {

        }
    }
}
