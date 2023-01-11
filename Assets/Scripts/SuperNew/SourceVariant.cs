
using Assets.WasapiAudio.Scripts.Wasapi;
using CSCore.CoreAudioAPI;

public class SourceVariant
{
    public WasapiCaptureType CaptureType { get; private  set; }
    public MMDevice Device { get; private set; }
    
    public string Name
    {
        get
        {
            switch (CaptureType)
            {
                case WasapiCaptureType.Loopback:
                    return "System sound";
                case WasapiCaptureType.Microphone:
                    return Device.FriendlyName.Split('(', ')')[1];
            }

            return "";
        }
    }
    
    public SourceVariant(MMDevice device = null)
    {
        Device = device;
        if (device==null)
        {
            CaptureType = WasapiCaptureType.Loopback;
        }
        else
        {
            CaptureType = WasapiCaptureType.Microphone;
        }
    }
}
