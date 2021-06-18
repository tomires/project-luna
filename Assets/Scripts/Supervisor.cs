using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Net;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Supervisor : MonoSingleton<Supervisor>
{
    [Header("Device Selection")]
    [SerializeField] private GameObject deviceSelectionPanel;
    [SerializeField] private GameObject deviceButtonPrefab;
    [SerializeField] private Transform deviceButtonParent;

    [Header("Supervisor")]
    [SerializeField] private GameObject supervisorPanel;

    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private List<IPEndPoint> discoveredServers = new List<IPEndPoint>();
    private LineRenderer lineRenderer;
    private Transform povCamera;
    private Coroutine drawingCoroutine;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, loadSceneMode) => OnSceneLoad(scene);

        lineRenderer = GetComponent<LineRenderer>();
        networkManager = FindObjectOfType<NetworkManager>();
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnServerFound);
        networkDiscovery.StartDiscovery();
    }

    private IEnumerator DrawLine()
    {
        while(true)
        {
            var position = lineRenderer.positionCount++;
            lineRenderer.SetPosition(position, povCamera.position);
            yield return new WaitForSeconds(Constants.LineRefreshPeriod);
        }
    }

    private void ResetLine()
    {
        lineRenderer.positionCount = 0;
    }

    private void OnServerFound(ServerResponse response)
    {
        if (discoveredServers.Contains(response.EndPoint)) return;
        discoveredServers.Add(response.EndPoint);

        Instantiate(deviceButtonPrefab, deviceButtonParent)
            .GetComponent<DeviceButton>()
            .Initialize(response.uri, ConnectToServer);
    }

    private void ConnectToServer(Uri uri)
    {
        networkManager.StartClient(uri);
        deviceSelectionPanel.SetActive(false);
        supervisorPanel.SetActive(true);
    }

    private void OnSceneLoad(Scene scene)
    {
        if (scene.name != Constants.EnvironmentScene) return;

        povCamera = GameObject.Find(Constants.PovCameraName).transform;
        drawingCoroutine = StartCoroutine(DrawLine());
    }
}
