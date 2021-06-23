using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class TestSubject : MonoSingleton<TestSubject>
{
    [SerializeField] private GameObject statePrefab;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private GameObject vrCamera;
    private GameObject povCamera;
    private bool hostEntered = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, loadSceneMode) => OnSceneLoad(scene);

        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.StartDiscovery();

        networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
        NetworkServer.RegisterHandler<ReadyMessage>(OnClientReady);

        /* VR Camera is the only camera we have with AudioListener attached to it */
        vrCamera = FindObjectOfType<AudioListener>().gameObject;
    }

    private void OnSceneLoad(Scene scene)
    {
        if (scene.name != Constants.EnvironmentScene) return;
        StartCoroutine(SynchronizePovCamera());
    }

    private void OnClientReady(NetworkConnection conn, ReadyMessage msg)
    {
        if(!hostEntered)
        {
            hostEntered = true;
            return;
        }

        Debug.Log("Client joined: " + conn);
        NetworkServer.SetClientReady(conn);
        GameObject state = Instantiate(statePrefab);
        NetworkServer.Spawn(state);
    }

    private IEnumerator SynchronizePovCamera()
    {
        yield return new WaitForSeconds(1.0f);
        povCamera = CameraReference.Instance.PovCamera.gameObject;

        while (true)
        {
            povCamera.transform.position = vrCamera.transform.position;
            povCamera.transform.rotation = vrCamera.transform.rotation;
            yield return null;
        }
    }
}
