using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PresetSquare : MonoBehaviour, IDragHandler, IBeginDragHandler, IDropHandler
{

    [SerializeField]
    private GameObject Frame;

    [SerializeField]
    private Image BackColor, Icon;

    private KineticPreset preset;

    public KineticPreset Preset{
        get
        {
            return preset;
        }
    }

    public void Init(KineticPreset preset)
    {

        if (this.preset!=null)
        {
            this.preset.color.RemoveListener(ColorChanged);
            this.preset.IconId.RemoveListener(IconChanged);
            this.preset.color2.RemoveListener(ColorChanged);
        }

        this.preset = preset;

        if (preset!=null)
        {
            preset.color.AddListener(ColorChanged);
            preset.IconId.AddListener(IconChanged);
            preset.color2.AddListener(ColorChanged);
        }

        BackColor.enabled = preset != null;
        Icon.enabled = preset != null;
    }

    private void ColorChanged(string v)
    {
        BackColor.color = preset.Color;
        Icon.color = preset.Color2;
    }

    private void IconChanged(int v)
    {
        Icon.sprite = preset.Icon;
    }

    public void SetSelected(bool v)
    {
        Frame.SetActive(v);
    }

    public void Random()
    {
        Debug.Log("Rand");

        preset.IconId.SetState(UnityEngine.Random.Range(0, DefaultResources.PresetSprites.Count-1));
        preset.Color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.3f, 0.3f, 1f, 1f);

        preset.Color2 = UnityEngine.Random.ColorHSV(0f, 1f,1f, 1f, 1f, 1f, 1f, 1f);

    }

    private void OnDestroy()
    {
        if (preset!=null)
        {
            preset.color.RemoveListener(ColorChanged);
            preset.IconId.RemoveListener(IconChanged);
            preset.color2.RemoveListener(ColorChanged);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (preset != null)
        {
            PresetsGrid.Instance.DraggingPreset.SetState(preset);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        PresetsGrid.Instance.DropPreset(this);
    }
}
