using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(Logger))]
public class State : NetworkBehaviour
{
    public Action<int> TimeTicked;
    public Action<int> OnCollision;
    public Action<int> OnRoomChanged;
    public Action<float> OnLuminanceLowerBoundChanged;
    public Action<float> OnLuminanceUpperBoundChanged;
    public Action<float> OnCurrentLuminanceChanged;
    public Action OnExperimentEnded;

    [SyncVar(hook = nameof(SecondsSinceStartChanged))]
    public int SecondsSinceStart = 0;

    [SyncVar(hook = nameof(CollisionCountChanged))]
    public int CollisionCount = 0;

    [SyncVar(hook = nameof(RoomChanged))]
    public int CurrentRoom = 0;

    [SyncVar(hook = nameof(LuminanceLowerBoundChanged))]
    public float LuminanceLowerBound = 0;

    [SyncVar(hook = nameof(LuminanceUpperBoundChanged))]
    public float LuminanceUpperBound = 0;

    [SyncVar(hook = nameof(CurrentLuminanceChanged))]
    public float CurrentLuminance = 0;

    private Logger logger;
    private bool logging = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        logger = GetComponent<Logger>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(UpdateSeconds());
    }

    private IEnumerator UpdateSeconds()
    {
        while(true)
        {
            SecondsSinceStart++;
            yield return new WaitForSecondsRealtime(1.0f);
        }
    }

    public void ChangeLevel()
    {
        if(!logging)
        {
            logger.StartLogging();
            logging = true;
        }

        SecondsSinceStart = 0;
        CollisionCount = 0;
        CurrentRoom += 1;
        ArrowManager.Instance.ChangeArrowDirection();
        logger.ChangeRoom(CurrentRoom);
    }

    public void TriggerCollision()
    {
        CollisionCount++;
        logger.PassCollision(CollisionCount);
    }

    public void EndExperiment()
    {
        logger.StopLogging();
        EndExperimentClient();
    }

    [ClientRpc]
    public void EndExperimentClient()
    {
        OnExperimentEnded?.Invoke();
    }

    private void CollisionCountChanged(int oldCollisionCount, int newCollisionCount)
    {
        OnCollision?.Invoke(newCollisionCount);
    }

    private void RoomChanged(int oldRoom, int newRoom)
    {
        OnRoomChanged?.Invoke(newRoom);
        AudioPlayer.Instance.PlaySound(Constants.Sounds.Confirmation);
        EnvironmentSwitcher.Instance.MoveToNextEnvironment();
    }

    private void SecondsSinceStartChanged(int oldSeconds, int newSeconds)
    {
        TimeTicked?.Invoke(newSeconds);
    }

    private void LuminanceLowerBoundChanged(float oldLuminance, float newLuminance)
    {
        OnLuminanceLowerBoundChanged?.Invoke(newLuminance);
    }

    private void LuminanceUpperBoundChanged(float oldLuminance, float newLuminance)
    {
        OnLuminanceUpperBoundChanged?.Invoke(newLuminance);
    }

    private void CurrentLuminanceChanged(float oldLuminance, float newLuminance)
    {
        OnCurrentLuminanceChanged?.Invoke(newLuminance);
    }
}
