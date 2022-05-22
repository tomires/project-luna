using System;
using UnityEngine;

public class Constants
{
    public const string EnvironmentScene = "Environment";
    public const float LineRefreshPeriod = 0.1f;
    public const float PositionLogPeriod = 0.02f;
    public const int PlaybackEnvironmentsPerRow = 3;
    public const float PlaybackEnvironmentSpacing = 0.5f;
    public const string LogExtension = "luna";
    public static string LogPath = $"{Application.persistentDataPath}/log_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.{LogExtension}";
    public static string InitializationGameObjectName = "Initialization";

    public class ScenePaths
    {
        public static string Initialization = "Assets/Initialization.unity";
        public static string Environment = "Assets/Environment.unity";
        public static string Supervisor = "Assets/Supervisor.unity";
        public static string TestSubject = "Assets/TestSubject.unity";
    }

    public class BuildPaths
    {
        public static readonly string testSubject = $"Build/Quest";
        public static readonly string supervisorAndroid = $"Build/Android";
        public static readonly string supervisorIOS = $"Build/iOS";
    }

    public class LogActions
    {
        public const string PositionLog = "M";
        public const string TimeTick = "T";
        public const string RoomChange = "R";
        public const string Collision = "C";
        public const string PositionLogPeriod = "P";
        public const string ExperimentEnd = "E";
        public const string EnvironmentOffset = "O";
        public const string IntensitySetting = "L";
    }
}
