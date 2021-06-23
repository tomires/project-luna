using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class State : NetworkBehaviour
{
    public Action<int> TimeTicked;
    public Action<int> OnCollision;
    public Action<int> OnRoomChanged;
    public Action OnExperimentEnded;

    [SyncVar(hook = nameof(SecondsSinceStartChanged))]
    public int SecondsSinceStart = 0;

    [SyncVar(hook = nameof(CollisionCountChanged))]
    public int CollisionCount = 0;

    [SyncVar(hook = nameof(RoomChanged))]
    public int CurrentRoom = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
            SecondsSinceStart += 1;
            yield return new WaitForSecondsRealtime(1.0f);
            CollisionCount++;
        }
    }

    public void ChangeLevel()
    {
        SecondsSinceStart = 0;
        CollisionCount = 0;
        CurrentRoom += 1;
    }

    private void CollisionCountChanged(int oldCollisionCount, int newCollisionCount)
    {
        OnCollision?.Invoke(newCollisionCount);
    }

    private void RoomChanged(int oldRoom, int newRoom)
    {
        OnRoomChanged?.Invoke(newRoom);
    }

    private void SecondsSinceStartChanged(int oldSeconds, int newSeconds)
    {
        TimeTicked?.Invoke(newSeconds);
    }
}
