using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetPreviewPanel : MonoBehaviour
{
    private KineticPreset preset;

    public KineticPreset Preset
    {
        get
        {
            return preset;
        }
    }

    [SerializeField]
    private PresetSquare Square;

    [SerializeField]
    private TMPro.TMP_InputField NameField;

    [SerializeField]
    private GameObject Frame;

    private void Start()
    {
        NameField.onEndEdit.AddListener(RenamePreset);
    }

    private void RenamePreset(string s)
    {
        preset.PresetName = s;
    }

    public void Init(KineticPreset preset)
    {
        this.preset = preset;
        NameField.text = preset.PresetName;
        Square.Init(preset);
    }

    public void SetSelected(bool v)
    {
        Frame.SetActive(v);
    }
}
