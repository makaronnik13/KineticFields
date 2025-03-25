using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class AudioVisualizationEffect : MonoBehaviour
    {
        // Inspector Properties
        public WasapiAudioSource WasapiAudioSource;
        public AudioVisualizationProfile Profile;
        public AudioVisualizationStrategy Strategy;

        public int SpectrumSize { get; private set; }

        public virtual void Awake()
        {
            if (WasapiAudioSource == null)
            {
                Debug.Log("You must set a WasapiAudioSource on "+gameObject);
                return;
            }

            SpectrumSize = WasapiAudioSource.SpectrumSize;
        }

        public float[] GetSpectrumData()
        {
            if (WasapiAudioSource == null)
            {
                Debug.Log("You must set a WasapiAudioSource");
                return null;
            }


            return WasapiAudioSource.GetSpectrumData(Strategy);
        }
    }
}
