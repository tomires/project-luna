using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class TestSubject : MonoSingleton<TestSubject>
{
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    void Start()
    {
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.StartDiscovery();

        networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }
}
