using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ButtonNetworkHelper : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;
    [SerializeField]
    private UnityTransport _unityTransport;

    private void Awake()
    {
        UpdateIpConnection();
    }

    public void StartServer()
    {
        if (_networkManager != null)
            _networkManager.StartServer();
    }

    public void StartHost()
    {
        if (_networkManager != null)
            _networkManager.StartHost();
    }

    public void StartClient()
    {
        if (_networkManager != null)
            _networkManager.StartClient();
    }

    public void ShutdownConnection()
    {
        if (_networkManager != null)
            _networkManager.Shutdown();
    }

    [ContextMenu("UpdateIpTest")]
    public void UpdateIpConnection()
    {
        _unityTransport.ConnectionData.Address = "10.26.1.79";
    }
}
