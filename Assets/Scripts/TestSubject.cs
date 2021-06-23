using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class TestSubject : MonoSingleton<TestSubject>
{
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private GameObject vrCamera;
    private GameObject povCamera;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, loadSceneMode) => OnSceneLoad(scene);

        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.StartDiscovery();

        networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();

        /* VR Camera is the only camera we have with AudioListener attached to it */
        vrCamera = FindObjectOfType<AudioListener>().gameObject;
    }

    private void OnSceneLoad(Scene scene)
    {
        if (scene.name != Constants.EnvironmentScene) return;
        StartCoroutine(SynchronizePovCamera());
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
