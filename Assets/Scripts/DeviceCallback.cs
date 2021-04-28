using CSCore.CoreAudioAPI;
using CSCore.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceCallback : IMMNotificationClient
{
    public void OnDeviceStateChanged(string deviceId, DeviceState deviceState)
    {
        Debug.Log(deviceId+"/"+deviceState);
    }

    public void OnDeviceAdded(string deviceId)
    {
        Debug.Log(deviceId+" add");
    }

    public void OnDeviceRemoved(string deviceId)
    {
        Debug.Log(deviceId + " remove");
    }

    public void OnDefaultDeviceChanged(DataFlow dataFlow, Role role, string deviceId)
    {
     
    }

    public void OnPropertyValueChanged(string deviceId, PropertyKey key)
    {
        Debug.Log(deviceId + " / "+key);
    }
}
