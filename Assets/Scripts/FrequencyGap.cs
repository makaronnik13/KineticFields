using com.armatur.common.flags;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FrequencyGap: ISource, ICloneable
{
    public enum GapType
    {
        Simple,
        Music,
        Envelope
    }

    public enum GapGroup
    {
        Main,
        Color,
        Rotation,
        Displacement,
        Additional
    }


    public string Name;
    public GapType GapT = FrequencyGap.GapType.Music;

    public GenericFlag<float> Position = new GenericFlag<float>("Position", 0);
    public GenericFlag<float> GapSize = new GenericFlag<float>("GapSize", 0);
    public GenericFlag<float> Multiplyer = new GenericFlag<float>("Sensivity", 100);
    public GenericFlag<float> Softness = new GenericFlag<float>("Softness", 1);

    private float lastValue;
    private float lastAverage;

    [SerializeField]
    private float[] _colorValues = new float[4] { 0,0,0,0};

    public Color color
    {
        get
        {
            return new Color(_colorValues[0],_colorValues[1],_colorValues[2], _colorValues[3]);
        }
        set
        {
            _colorValues[0] = value.r;
            _colorValues[1] = value.g;
            _colorValues[2] = value.b;
            _colorValues[3] = value.a;
        }
    }


    private Action<float> onValueChanged = (v) => { };
    public Action<float> OnValueChanged
    {
        get
        {
            return onValueChanged;
        }
        set
        {
            onValueChanged = value;
        }
    }

    public GenericFlag<float> BaseValue = new GenericFlag<float>("BaseValue", 0);

    [SerializeField]
    private float value;
    public float Value
    {
        get
        {
            return value;
        }
    }

    public List<float> Data = new List<float>();

    public float Start
    {
        get
        {
            return Mathf.Max(0, Position.Value-GapSize.Value/2f);
        }
    }

    public float End
    {
        get
        {
            return Mathf.Min(1, Position.Value + GapSize.Value / 2f);
        }
    }

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


    public float SourceValue
    {
        get
        {
            return Value;
        }
    }

    public Sprite Icon => Sprite;

    public FrequencyGap(string name, float pos, float size, Color color, Sprite sprite)
    {
        this.Name = name;
        Position.SetState(pos);
        GapSize.SetState(size);
        this.color = color;
        Sprite = sprite;
    }

    public void UpdateFrequency(List<float> data)
    {
        Data = data;
        float dataAverage = data.Average();
        float max = data.Max();
        lastAverage = Mathf.Lerp(lastAverage, dataAverage, Time.deltaTime*2f);

        float diff = max/2f - lastAverage/2f;


        if (max>lastValue)
        {
            lastValue = max;
        }
        else
        {
            lastValue -= Time.deltaTime;
        }

        /*

        switch (GapT)
        {
            case GapType.Simple:
                value = BaseValue.Value;
                break;
            case GapType.Music:
                value = dataAverage * Multiplyer.Value + BaseValue.Value;
                break;
            case GapType.Envelope:
                value = BaseValue.Value;
                break;
        }
        */

        value = diff * Multiplyer.Value + BaseValue.Value;

        OnValueChanged(value);
    }

    public object Clone()
    {
        FrequencyGap gap = new FrequencyGap(Name, Position.Value, GapSize.Value, color, Sprite);

        return gap;
    }
}