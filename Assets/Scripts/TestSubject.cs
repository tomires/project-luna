using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class TestSubject : MonoSingleton<TestSubject>
{
    private NetworkManager networkManager;

    void Start()
    {
        GetComponent<NetworkDiscovery>().StartDiscovery();

        networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartHost();
    }
}
