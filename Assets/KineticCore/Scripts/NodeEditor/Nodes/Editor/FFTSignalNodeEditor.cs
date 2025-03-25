using System.Linq;
using InputNode;
using ModestTree;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(FFTSignalNode))]
public class FFTSignalNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        FFTSignalNode signalNode = (FFTSignalNode)target;

        EditorGUILayout.BeginVertical();
        signalNode.SpectrumGap = (FrequencyGap)EditorGUILayout.EnumPopup("Spectrum Gap", signalNode.SpectrumGap);
        EditorGUILayout.MinMaxSlider(new GUIContent("Range"), ref signalNode.startSpectrumGap, ref signalNode.endSpectrumGap, 0f, 1f);
        float[] data = signalNode.OutputSpectrum.ToArray();

        if (!data.IsEmpty())
        {
            SpectrumDrawer.DrawGraph(data.ToArray(), data.Max(), data.Max());     
        }
       
        NodeEditorGUILayout.PortField(target.GetOutputPort("output"));
        EditorGUILayout.EndVertical();
    }
}