using System;
using UniRx;
using UnityEngine;

namespace KineticFields
{
    public class ParameterData
    {
        public string Name;
        public float Min;
        public float Max;
        public float MinGap = 0;
        public float MaxGap = 1;
        public float Multiplyer = 1;
        public int Beats = 1;
        public ReactiveProperty<float> Value { get; private set; } = new ReactiveProperty<float>(0.5f);
        public ReactiveProperty<SourceType> SourceType { get; private set; } = new ReactiveProperty<SourceType>(KineticFields.SourceType.None);

        public bool UseCurve = false;
        public AnimationCurve Curve;

        public ParameterData()
        {

        }

        public ParameterData(string name, float min = 0, float max = 1)
        {
            Name = name;
            this.Min = min;
            this.Max = max;
        }
    }
}
