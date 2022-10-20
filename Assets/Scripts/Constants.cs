using System;
using UnityEngine;

public class Constants
{
    public const string InitializationScene = "Initialization";
    public const string EnvironmentScene = "Environment";
    public const float LineRefreshPeriod = 0.1f;
    public const float PositionLogPeriod = 0.02f;
    public const int PlaybackEnvironmentsPerRow = 3;
    public const float PlaybackEnvironmentSpacing = 0.5f;
    public const string LogExtension = "luna";
    public static string LogPath = $"{Application.persistentDataPath}/log_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.{LogExtension}";
    public static string InitializationGameObjectName = "Initialization";
    public static string PathCliParameter = "-logPath";

    public class ScenePaths
    {
        public static string Initialization = "Assets/Initialization.unity";
        public static string Environment = "Assets/Environment.unity";
        public static string Supervisor = "Assets/Supervisor.unity";
        public static string TestSubject = "Assets/TestSubject.unity";
        public static string Playback = "Assets/Playback.unity";
    }

    public class BuildPaths
    {
        public static readonly string testSubject = $"Build/luna-quest.apk";
        public static readonly string supervisorAndroid = $"Build/luna-mobile.apk";
        public static readonly string supervisorIOS = $"Build/iOS";
        public static readonly string playbackWindows = $"Build/PlaybackWin/LunaPlayback.exe";
        public static readonly string playbackLinux = $"Build/PlaybackLinux/LunaPlayback";
        public static readonly string playbackMac = $"Build/PlaybackMac/LunaPlayback";
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

    public enum Sounds
    {
        Confirmation, Up, Down
    }
}
