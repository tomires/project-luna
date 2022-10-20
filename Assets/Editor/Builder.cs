using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Builder
{
    public static string[] LiveScenes = {
        Constants.ScenePaths.Initialization,
        Constants.ScenePaths.Environment,
        Constants.ScenePaths.Supervisor,
        Constants.ScenePaths.TestSubject,
    };

    public static string[] PlaybackScenes = {
        Constants.ScenePaths.Playback,
        Constants.ScenePaths.Environment,
    };

    [MenuItem("Build/Build TestSubject")]
    public static void BuildTestSubject()
    {
        SetSupervisorMode(false);
        string deployPath = Constants.BuildPaths.testSubject;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildPipeline.BuildPlayer(LiveScenes, deployPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build/Build Supervisor (Android)")]
    public static void BuildSupervisorAndroid()
    {
        SetSupervisorMode(true);
        string deployPath = Constants.BuildPaths.supervisorAndroid;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildPipeline.BuildPlayer(LiveScenes, deployPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build/Build Supervisor (iOS)")]
    public static void BuildSupervisorIOS()
    {
        SetSupervisorMode(true);
        string deployPath = Constants.BuildPaths.supervisorIOS;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        BuildPipeline.BuildPlayer(LiveScenes, deployPath, BuildTarget.iOS, BuildOptions.None);
    }

    [MenuItem("Build/Build Playback (Windows)")]
    public static void BuildPlaybackAppWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        BuildPipeline.BuildPlayer(PlaybackScenes, Constants.BuildPaths.playbackWindows, 
            BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Build/Build Playback (Linux)")]
    public static void BuildPlaybackAppLinux()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
        BuildPipeline.BuildPlayer(PlaybackScenes, Constants.BuildPaths.playbackLinux,
            BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    [MenuItem("Build/Build Playback (macOS)")]
    public static void BuildPlaybackAppMac()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        BuildPipeline.BuildPlayer(PlaybackScenes, Constants.BuildPaths.playbackMac,
            BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    private static void SetSupervisorMode(bool enabled)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Constants.ScenePaths.Initialization);
        GameObject.Find(Constants.InitializationGameObjectName).GetComponent<Initializer>().supervisor = enabled;
        EditorSceneManager.SaveOpenScenes();
    }
}
