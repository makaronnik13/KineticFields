using System;
using UnityEngine;

public interface ISource
{
    Sprite Icon { get; }
    Action<float> OnValueChanged { get; set; }
    float SourceValue { get;}

}