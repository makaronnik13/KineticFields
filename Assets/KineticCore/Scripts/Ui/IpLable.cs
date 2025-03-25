using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class IpLable : MonoBehaviour
{
    public TMP_Text ipText; // Присвой сюда TMP_Text в инспекторе

    void Start()
    {
        string localIP = GetLocalIPAddress();
        if (ipText != null)
        {
            ipText.text = "Local IP: " + localIP;
        }
        else
        {
            Debug.LogError("TMP_Text не назначен в инспекторе!");
        }
    }

    string GetLocalIPAddress()
    {
        string localIP = "Не найден";

        try
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка получения IP: " + ex.Message);
        }

        return localIP;
    }
}