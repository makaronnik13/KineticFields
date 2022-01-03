using UnityEngine;
using Zenject;

public class StartSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AudioDecorator>().AsSingle();
    }
}