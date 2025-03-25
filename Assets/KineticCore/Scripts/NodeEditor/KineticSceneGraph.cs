using System;
using System.Collections.Generic;
using XNode;
using UniRx;
using UnityEngine;
using KineticFields;
using Zenject;
using System.Linq;
using ComponentsNodes;
using InputNode;
using OutputNodes;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

public class KineticSceneGraph : SceneGraph<KineticGraph>
{
    private FFTService _fftService;

    [SerializeField]
    public List<Object> sceneReferences = new List<Object>();

    [Inject]
    void Construct(FFTService fftService, ConstantBPMSource bpmService, KineticInputService inputService)
    {
        _fftService = fftService;
        
        inputService.OnItemsLoaded.Subscribe(_ =>
        {
            foreach (InputSONode inputNode in graph.GetNodes<InputSONode>())
            {
                inputNode.InitInstance(inputService.GetItemInstance(inputNode.inputSO));
            }
            InitializeComponentNodes<Transform>();
            InitializeComponentNodes<Renderer>();
            InitializeComponentNodes<VisualEffect>();
            StartGraph();
        }).AddTo(this);

        bpmService.Bpm.Subscribe(bpm =>
        {
            SetBpm(bpm);
        }).AddTo(this);
        
        bpmService.OnResync.Subscribe(bpm =>
        {
            RestartBeat(bpmService.Bpm.Value);
        }).AddTo(this);
        
    }

    private void RestartBeat(int bpm)
    {
        foreach (OscillatorNode node in graph.GetNodes<OscillatorNode>())
        {
            node.SetBpm(bpm, true);
        }
    }
    
    private void SetBpm(int bpm)
    {
        foreach (OscillatorNode node in graph.GetNodes<OscillatorNode>())
        {
            node.SetBpm(bpm, false);
        }
    }

    private void Update()
    {
        foreach (var node in graph.nodes)
        {
            if (node is IUpdatableNode updatableNode)
            {
                updatableNode.UpdateNode();
            }
        }
        
        foreach (FFTSignalNode fftNode in graph.GetNodes<FFTSignalNode>())
        {
            fftNode.UpdateNode(_fftService.GetSpectrumGap(fftNode.SpectrumGap).ToList());
        }
    }

    public List<T> GetRefComponents<T>()
    {
        return sceneReferences.OfType<T>().ToList();
    }

    private void InitializeComponentNodes<T>()
    {
        foreach (var node in graph.GetNodes<ComponentNode<T>>().Select(n => n as ComponentNode<T>))
        {
            var matchingComponent = sceneReferences
                .OfType<T>()
                .FirstOrDefault(obj => (obj as Object).GetInstanceID() == node._componentId);

            if (matchingComponent != null)
            {
                node.SetComponent(matchingComponent);
            }
        }
    }
    
    private void OnDisable()
    {
        foreach (var node in graph.nodes)
        {
            if (node is IDisposable reactiveNode)
            {
                reactiveNode.Dispose();
            }
        }
    }
}
