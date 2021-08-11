using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class Logger : MonoSingleton<Logger>
{
    private FileStream logFile;
    private Coroutine loggingRoutine;
    private Transform povCamera;
    private Transform environment;
    private float roomTime;
    private int lastTimeTick;

    public void StartLogging()
    {
        povCamera = CameraReference.Instance.PovCamera.transform;
        environment = Calibrator.Instance.transform;
        loggingRoutine = StartCoroutine(LoggingRoutine());
    }

    public void StopLogging()
    {
        if(loggingRoutine != null)
            StopCoroutine(loggingRoutine);
        LogExperimentEnd();
    }

    public void ChangeRoom(int room)
    {
        LogRoomChange(room);
        roomTime = 0f;
        lastTimeTick = 0;
    }

    public void PassCollision(int collisionCount)
        => LogCollision(collisionCount);

    private IEnumerator LoggingRoutine()
    {
        string path = Constants.LogPath;
        if (File.Exists(path))
            File.Delete(path);

        logFile = File.Create(path);
        var timePeriod = Constants.PositionLogPeriod;
        LogPositionLogPeriod(timePeriod);
        LogLightningIntensity(Calibrator.Instance.MinLightIntensity, Calibrator.Instance.MaxLightIntensity);
        LogEnvironmentOffset();

        while (true)
        {
            LogPosition();
            CheckTimeTick();

            roomTime += timePeriod;
            yield return new WaitForSecondsRealtime(timePeriod);
        }
    }

    private void CheckTimeTick()
    {
        var roomTimeInt = Mathf.FloorToInt(roomTime);
        if (roomTimeInt != lastTimeTick)
        {
            lastTimeTick = roomTimeInt;
            LogTimeTick(lastTimeTick);
        }
    }

    private void LogPosition()
        => AppendText(Constants.LogActions.PositionLog, 
            $"{povCamera.localPosition.ToString()}:{povCamera.localRotation.eulerAngles.ToString()}");

    private void LogPositionLogPeriod(float timePeriod)
        => AppendText(Constants.LogActions.PositionLogPeriod,
            timePeriod.ToString());

    private void LogEnvironmentOffset()
        => AppendText(Constants.LogActions.EnvironmentOffset,
            $"{environment.position.ToString()}:{environment.rotation.eulerAngles.ToString()}");

    private void LogExperimentEnd()
        => AppendText(Constants.LogActions.ExperimentEnd, "");

    private void LogRoomChange(int room)
        => AppendText(Constants.LogActions.RoomChange,
            room.ToString());

    private void LogTimeTick(int time)
        => AppendText(Constants.LogActions.TimeTick,
            time.ToString());

    private void LogCollision(int collisionCount)
        => AppendText(Constants.LogActions.Collision,
            collisionCount.ToString());

    private void LogLightningIntensity(float minIntensity, float maxIntensity)
        => AppendText(Constants.LogActions.IntensitySetting,
            $"{minIntensity}:{maxIntensity}");

    private void AppendText(string action, string body)
    {
        byte[] info = new UTF8Encoding(true).GetBytes($"{action}:{body}\n");
        logFile.Write(info, 0, info.Length);
    }
}
