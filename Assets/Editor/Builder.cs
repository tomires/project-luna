using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Builder
{
    public static string[] Scenes = {
        Constants.ScenePaths.Initialization,
        Constants.ScenePaths.Environment,
        Constants.ScenePaths.Supervisor,
        Constants.ScenePaths.TestSubject,
    };

    [MenuItem("Build/Build TestSubject")]
    public static void BuildTestSubject()
    {
        SetSupervisorMode(false);
        string deployPath = Constants.BuildPaths.testSubject;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildPipeline.BuildPlayer(Scenes, deployPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build/Build Supervisor (Android)")]
    public static void BuildSupervisorAndroid()
    {
        SetSupervisorMode(true);
        string deployPath = Constants.BuildPaths.supervisorAndroid;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildPipeline.BuildPlayer(Scenes, deployPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build/Build Supervisor (iOS)")]
    public static void BuildSupervisorIOS()
    {
        SetSupervisorMode(true);
        string deployPath = Constants.BuildPaths.supervisorIOS;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        BuildPipeline.BuildPlayer(Scenes, deployPath, BuildTarget.iOS, BuildOptions.None);
    }

    private static void SetSupervisorMode(bool enabled)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Constants.ScenePaths.Initialization);
        GameObject.Find(Constants.InitializationGameObjectName).GetComponent<Initializer>().supervisor = true;
        EditorSceneManager.SaveOpenScenes();
    }
}
