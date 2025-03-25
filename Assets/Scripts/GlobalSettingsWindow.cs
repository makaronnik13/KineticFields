using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class GlobalSettingsWindow : MonoBehaviour
{
    [SerializeField]
    private VisualEffect Visual;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private TMPro.TMP_Dropdown  Gradient, SizeOverLifetime, Mesh;

    [SerializeField]
    private TMPro.TMP_InputField CountPerFrame, MinLifetime, MaxLifetime;

    private void Start()
    {

        Gradient.onValueChanged.AddListener(GradientChanged);
        SizeOverLifetime.onValueChanged.AddListener(SizeOverLifetimeChanged);
        Mesh.onValueChanged.AddListener(MeshChanged);
        
        /*
   ColorTexture.ClearOptions();

   for (int i = 0; i < DefaultResources.Settings.ColorMaps.Count; i++)
   {
       Sprite newSprite = Sprite.Create(DefaultResources.Settings.ColorMaps[i], new Rect(Vector2.zero, new Vector2(DefaultResources.Settings.ColorMaps[i].width, DefaultResources.Settings.ColorMaps[i].height)), Vector2.one / 2f);
       Debug.Log(newSprite);
       ColorTexture.AddOptions(new List<TMPro.TMP_Dropdown.OptionData>() {new TMPro.TMP_Dropdown.OptionData(DefaultResources.Settings.ColorMaps[i].name, newSprite)});
   }
 

        DisplacementTexture.ClearOptions();
        for (int i = 0; i < DefaultResources.Settings.DisplacementMaps.Count; i++)
        {
            Sprite newSprite = Sprite.Create(DefaultResources.Settings.DisplacementMaps[i], new Rect(Vector2.zero, new Vector2(DefaultResources.Settings.DisplacementMaps[i].width, DefaultResources.Settings.DisplacementMaps[i].height)), Vector2.one / 2f);
            Debug.Log(newSprite);
            DisplacementTexture.AddOptions(new List<TMPro.TMP_Dropdown.OptionData>() { new TMPro.TMP_Dropdown.OptionData(DefaultResources.Settings.DisplacementMaps[i].name, newSprite) });
        }
        */

        Gradient.ClearOptions();
        for (int i = 0; i < DefaultResources.Settings.Gradients.Count; i++)
        {
            Gradient.AddOptions(new List<string>() { "G_" + i });
        }
        SizeOverLifetime.ClearOptions();
        for (int i = 0; i < DefaultResources.Settings.SizeCurves.Count; i++)
        {
            SizeOverLifetime.AddOptions(new List<string>() { "S_" + i });
        }

        Mesh.ClearOptions();
        Mesh.AddOptions(DefaultResources.Settings.Meshes.Select(m => m.name).ToList());



        CountPerFrame.onSubmit.AddListener(CountChanged);
        
        MinLifetime.onSubmit.AddListener(LifetimeChanged);
        MaxLifetime.onSubmit.AddListener(LifetimeChanged);


        Gradient.value = 0;
        SizeOverLifetime.value = 0;
        Mesh.value = 0;
        CountPerFrame.text = 200000.ToString();
        MinLifetime.text = 0.2f.ToString();
        MaxLifetime.text = 1f.ToString();
    }


    private void GradientChanged(int v)
    {
        Visual.SetGradient("ColorOverLifetime", DefaultResources.Settings.Gradients[v]);
    }

    private void SizeOverLifetimeChanged(int v)
    {
        Visual.SetAnimationCurve("Scale", DefaultResources.Settings.SizeCurves[v]);
    }

    private void MeshChanged(int v)
    {
        Visual.SetMesh("ParticleMesh", DefaultResources.Settings.Meshes[v]);
    }

    private void CountChanged(string v)
    {
        Visual.SetInt("Rate", int.Parse(v));

    }

    private void LifetimeChanged(string arg0)
    {
        Visual.SetFloat("MinLefitime", float.Parse(MinLifetime.text));
        Visual.SetFloat("MaxLifetime", float.Parse(MaxLifetime.text));
    }


    public void ToggleWindow()
    {
        View.SetActive(!View.activeInHierarchy);
        if (View.activeInHierarchy)
        {
            
        }
    }
}
