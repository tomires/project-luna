using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Net;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.IO;

[RequireComponent(typeof(LineRenderer))]
public class Supervisor : MonoSingleton<Supervisor>
{
    [Header("Device Selection")]
    [SerializeField] private GameObject deviceSelectionPanel;
    [SerializeField] private GameObject deviceButtonPrefab;
    [SerializeField] private Transform deviceButtonParent;

    [Header("Supervisor")]
    [SerializeField] private GameObject supervisorPanel;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text roomText;
    [SerializeField] private GameObject collisionPrefab;

    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private State state;
    private List<IPEndPoint> discoveredServers = new List<IPEndPoint>();
    private LineRenderer lineRenderer;
    private Transform povCamera;
    private Transform topDownCamera;
    private Coroutine drawingCoroutine;
    private List<GameObject> spawnedCollisionPrefabs = new List<GameObject>();

    private FileStream logFile;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, loadSceneMode) => OnSceneLoad(scene);

        lineRenderer = GetComponent<LineRenderer>();
        networkManager = FindObjectOfType<NetworkManager>();
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnServerFound);
        networkDiscovery.StartDiscovery();

        StartCoroutine(LoggingRoutine());
    }

    private void Update()
    {
        if (CameraReference.Instance) {
            transform.position = CameraReference.Instance.TopDownCamera.transform.position;
            transform.rotation = CameraReference.Instance.TopDownCamera.transform.rotation;
        }
    }

    private IEnumerator DrawLine()
    {
        //transform.SetParent(CameraReference.Instance.TopDownCamera.transform);
        yield return new WaitForSeconds(1.0f);
        povCamera = CameraReference.Instance.PovCamera.transform;
        povCamera.gameObject.SetActive(true);
        while (true)
        {
            var position = lineRenderer.positionCount++;
            lineRenderer.SetPosition(position, povCamera.position);
            yield return new WaitForSeconds(Constants.LineRefreshPeriod);
        }
    }

    private void ResetLine()
    {
        //StopCoroutine(drawingCoroutine);
        lineRenderer.positionCount = 0;

        foreach (var collisionPrefab in spawnedCollisionPrefabs)
            Destroy(collisionPrefab);
        spawnedCollisionPrefabs.Clear();

        //drawingCoroutine = StartCoroutine(DrawLine());
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
        drawingCoroutine = StartCoroutine(DrawLine());
        StartCoroutine(CreateStateHooks());
    }

    private IEnumerator CreateStateHooks()
    {
        while(state == null)
        {
            state = FindObjectOfType<State>();
            yield return new WaitForSecondsRealtime(0.2f);
        }
        state.OnCollision = UpdateCollisionCount;
        state.TimeTicked = UpdateTime;
        state.OnRoomChanged = UpdateRoom;
        state.OnExperimentEnded = EndExperiment;
    }

    private void UpdateCollisionCount(int collisionCount)
    {
        collisionCountText.text = collisionCount.ToString();
        var collisionIndicator = Instantiate(collisionPrefab, povCamera.position, Quaternion.identity);
        spawnedCollisionPrefabs.Add(collisionIndicator);
        AppendText($"C:{collisionCount.ToString()}");
    }

    private void UpdateTime(int seconds)
    {
        timeText.text = $"{Mathf.Floor(seconds / 60):00}:{seconds % 60:00}";
        AppendText($"T:{seconds.ToString()}");
    }

    private void UpdateRoom(int newRoom)
    {
        roomText.text = newRoom.ToString();
        ArrowManager.Instance.ChangeArrowDirection();
        ResetLine();
        AppendText($"R:{newRoom.ToString()}");
    }

    private void EndExperiment()
    {
        Debug.Log("BOO");
        logFile.Close();
    }

    private IEnumerator LoggingRoutine()
    {
        string path = $"{Application.persistentDataPath}/log_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.txt";

        if (File.Exists(path))
            File.Delete(path);

        logFile = File.Create(path);

        while (true)
        {
            yield return new WaitForSecondsRealtime(Constants.PositionLogPeriod);
            if(povCamera != null)
                AppendText($"M:{povCamera.position.ToString()}:{povCamera.rotation.eulerAngles.ToString()}");
        }
    }

    private void AppendText(string text)
    {
        byte[] info = new UTF8Encoding(true).GetBytes($"{text}\n");
        logFile.Write(info, 0, info.Length);
    }
}
