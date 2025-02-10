using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstallerCircle : MonoInstaller
{
    [SerializeField]
    private ConstantBPMSource bpmSource;

    public override void InstallBindings()
    {
        base.InstallBindings();

        Container.Bind<ConstantBPMSource>().FromInstance(bpmSource);
    }
}
