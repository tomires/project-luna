using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class State : NetworkBehaviour
{
    public Action<int> TimeTicked;
    public Action OnCollision;

    [SyncVar(hook = "SecondsSinceStartChanged")]
    public int SecondsSinceStart = 0;

    [SyncVar]
    public int CollisionCount = 0;

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
            Debug.Log("A");
            CollisionCount++;
            TriggerCollision();
        }
    }

    [ClientRpc]
    public void ChangeLevel()
    {
        Debug.Log("LVL UP");
    }

    [ClientRpc]
    public void TriggerCollision()
    {
        Debug.Log("COLLISION");
        OnCollision?.Invoke();
    }

    private void SecondsSinceStartChanged(int oldSeconds, int newSeconds)
    {
        TimeTicked?.Invoke(newSeconds);
    }
}
