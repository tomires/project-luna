using Mirror;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playback : MonoBehaviour
{
    [SerializeField] private RenderTexture outputTexture;
    private List<LineRenderer> LineRenderers;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, loadSceneMode) => OnSceneLoad(scene);

        var networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartHost();
    }

    private void OnSceneLoad(Scene scene)
    {
        if (scene.name != Constants.EnvironmentScene) return;
        EnvironmentSwitcher.Instance.SwitchToPlaybackLayout();
        GetLineRenderers();
        StandaloneFileBrowser.OpenFilePanelAsync("Select log to process", "", Constants.LogExtension, false, PlayLog);
    }

    private void PlayLog(string[] paths)
    {
        if (paths.Length == 0)
        {
            Application.Quit();
            return;
        }

        var path = paths[0];
        var room = 0;
        var time = 0;

        using (var reader = File.OpenText(path))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Split(':');

                switch(line[0])
                {
                    case Constants.LogActions.PositionLog:
                        if(room != 0) RenderPosition(room - 1, Utils.DeserializeVector3(line[1]));
                        break;
                    case Constants.LogActions.TimeTick:
                        break;
                    case Constants.LogActions.RoomChange:
                        room = int.Parse(line[1]);
                        break;
                    case Constants.LogActions.Collision:
                        break;
                    default:
                        break;
                }
            }
        }

        StartCoroutine(SaveLogIntoFile(path.Replace($".{Constants.LogExtension}", ".png")));
    }

    private IEnumerator SaveLogIntoFile(string path)
    {
        yield return new WaitForEndOfFrame();
        var data = outputTexture.ToTexture2D().EncodeToPNG();
        File.WriteAllBytes(path, data);
        Application.Quit();
    }

    private void GetLineRenderers()
    {
        LineRenderers = new List<LineRenderer>();
        foreach (var room in EnvironmentSwitcher.Instance.Environments)
            LineRenderers.Add(room.GetComponentInChildren<LineRenderer>());
    }

    private void RenderPosition(int room, Vector3 position)
    {
        var lineRenderer = LineRenderers[room];
        var offset = lineRenderer.transform.position;
        var pos = lineRenderer.positionCount++;
        lineRenderer.SetPosition(pos, position + offset - 4 * Vector3.forward + 1.25f * Vector3.right);
    }

    private void RenderCollision()
    {

    }
}
