using XNode;
using UniRx;
using UnityEngine;
using KineticFields;
using Zenject;
using System.Linq;
using System.Collections;
using KineticFields;
using XNode;

public class KineticSceneGraph : SceneGraph<KineticGraph>
{

    private FFTService _fftService;
    private ConstantBPMSource _bpmService;

    [Inject]
    void Construct(FFTService fftService, ConstantBPMSource bpmService, KineticInputService inputService)
    {
        _fftService = fftService;
        _bpmService = bpmService;

        
        fftService.GetCachedSpectrumGap(FrequencyGap.Presence);

        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            Debug.Log("CREATE INSTANCES!!!");
            foreach (InputSONode inputNode in graph.GetNodes<InputSONode>())
            {
                inputNode.InitInstance(inputService.GetItemInstance(inputNode.inputSO));
            }
            StartGraph();
        }).AddTo(this);
    }

    private void Update()
    {
        foreach (FFTSignalNode fftNode in graph.GetNodes<FFTSignalNode>()) 
        {

            fftNode.SetValues(_fftService.GetSpectrumGap(fftNode.SpectrumGap).ToList());
        }
    }
}