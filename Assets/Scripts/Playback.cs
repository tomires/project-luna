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
    [SerializeField] private GameObject collisionPrefab;
    [SerializeField] private GameObject stampPrefab;
    private List<LineRenderer> LineRenderers;
    private Vector3 environmentOffsetPosition;
    private Quaternion environmentOFfsetRotation;

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
                    case Constants.LogActions.EnvironmentOffset:
                        environmentOffsetPosition = Utils.DeserializeVector3(line[1]);
                        environmentOFfsetRotation = Quaternion.Euler(Utils.DeserializeVector3(line[2]));
                        break;
                    case Constants.LogActions.TimeTick:
                        time = int.Parse(line[1]);
                        break;
                    case Constants.LogActions.RoomChange:
                        if(room != 0)
                            DisplayRoomTime(room - 1, time);
                        room = int.Parse(line[1]);
                        break;
                    case Constants.LogActions.Collision:
                        RenderCollision(room - 1);
                        break;
                    case Constants.LogActions.ExperimentEnd:
                        DisplayRoomTime(room - 1, time);
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
        var offset = lineRenderer.transform.position - 4 * Vector3.forward;
        var pos = lineRenderer.positionCount++;
        lineRenderer.SetPosition(pos, 
            Quaternion.Inverse(environmentOFfsetRotation) * (position - environmentOffsetPosition) + offset);
    }

    private void RenderCollision(int room)
    {
        var lineRenderer = LineRenderers[room];
        Instantiate(collisionPrefab, lineRenderer.GetPosition(lineRenderer.positionCount - 1), Quaternion.identity);
    }

    private void DisplayRoomTime(int room, int time)
    {
        var stamp = Instantiate(stampPrefab);
        stamp.GetComponent<TextMesh>().text = $"Room {room}\n{time}s";
        stamp.transform.position += LineRenderers[room].transform.position;
    }
}
