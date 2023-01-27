using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using Zenject;

namespace KineticFields
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField]
        private AudioVisualizationProfile fftProfile;

        public override void InstallBindings()
        {
            Debug.Log("Install project bindings");

            Container.BindFactory<ParameterData, ParameterInstance, ParameterInstance.Factory>();
            Container.BindWithInterfaces<PrefabCreator>();
           // Container.BindWithInterfaces<FFTService>();
            Container.Bind<AudioVisualizationProfile>().FromInstance(fftProfile).AsSingle().NonLazy();
        }
    }
}
