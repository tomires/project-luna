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

    public class LogActions
    {
        public const string PositionLog = "M";
        public const string TimeTick = "T";
        public const string RoomChange = "R";
        public const string Collision = "C";
        public const string PositionLogPeriod = "P";
        public const string ExperimentEnd = "E";
        public const string EnvironmentOffset = "O";
    }
}
