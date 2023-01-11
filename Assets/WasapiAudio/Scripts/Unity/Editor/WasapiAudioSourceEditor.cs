using System.Linq;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WasapiAudioSource))]
[CanEditMultipleObjects]
public class WasapiAudioSourceEditor : Editor
{
    public override bool RequiresConstantRepaint() => Application.isPlaying;
    
    private WasapiAudioSource source;
    
    void OnEnable()
    {
        source = target as WasapiAudioSource;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            float[] data = source.GetSpectrumData(AudioVisualizationStrategy.Scaled, source.Profile);
            //Debug.Log(data.Sum());
            if (data.Length != 0)
            {
                SpectrumDrawer.DrawGraph(data);
            }
        }
    }
}
