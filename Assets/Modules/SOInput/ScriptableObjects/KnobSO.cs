using UnityEngine;

[CreateAssetMenu(fileName = "Knob", menuName = "InputSo/Knob")]
public class KnobSO : InputSO
{
    public float MinValue = 0f;
    public float MaxValue = 1f;

    public override void SetValue(float value)
    {
        base.SetValue(Mathf.Clamp(value, MinValue, MaxValue));
    }
}