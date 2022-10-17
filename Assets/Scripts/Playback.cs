using Mirror;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playback : MonoBehaviour
{
    [SerializeField] private bool showEnvironment = true;
    [SerializeField] private RenderTexture outputTexture;
    [SerializeField] private TextMesh metadataText;
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

    private async void OnSceneLoad(Scene scene)
    {
        if (scene.name != Constants.EnvironmentScene) return;
        await Task.Delay(100);
        EnvironmentSwitcher.Instance.SwitchToPlaybackLayout();
        GetLineRenderers();

        if (Application.isEditor)
            ShowLogSelectionPrompt();
        else
        {
            var readingPathArg = false;
            var path = string.Empty;

            foreach (var arg in System.Environment.GetCommandLineArgs())
            {
                if (arg == Constants.PathCliParameter)
                    readingPathArg = true;
                else if (readingPathArg)
                {
                    path = arg;
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
                ShowLogSelectionPrompt();
            else
                PlayLog(new string[] { path });
        }
    }

    private void ShowLogSelectionPrompt()
    {
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
                        if (room != 0)
                            DisplayRoomTime(room - 1, time);
                        room = int.Parse(line[1]);
                        break;
                    case Constants.LogActions.Collision:
                        if (room != 0)
                            RenderCollision(room - 1);
                        break;
                    case Constants.LogActions.IntensitySetting:
                        DisplayMetadata(Utils.GetFilename(path), float.Parse(line[1]), float.Parse(line[2]));
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
        if (!showEnvironment) HideEnvironment();
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

    private void DisplayMetadata(string filename, float minIntensity, float maxIntensity)
    {
        metadataText.text = $"{filename}\nLmin: {minIntensity}\nLmax: {maxIntensity}";
    }

    private void HideEnvironment()
    {
        foreach (var renderer in FindObjectsOfType<MeshRenderer>())
            renderer.enabled = false;
    }
}
