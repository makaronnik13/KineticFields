using System;
using UniRx;
using UnityEngine;

public class Source
{
    public ReactiveProperty<float> Value = new ReactiveProperty<float>(0);
    public float minValue = 0;
    public float maxValue = 0;
    public string Name;

    [SerializeField]
    private int spriteId = -1;

    public Sprite Sprite
    {
        get
        {
            return DefaultResources.GapSprites[spriteId];
        }
        set
        {
            spriteId = DefaultResources.GapSprites.IndexOf(value);
        }
    }

    public Color Color;

    public Source(string name, Sprite icon, float minValue, float maxValue, Color color)
    {
        this.Color = color;
        this.Name = name;
        this.Sprite = icon;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public virtual void SetValue(float v)
    {
        Value.Value = Mathf.Clamp(minValue, maxValue, v);
    }
}