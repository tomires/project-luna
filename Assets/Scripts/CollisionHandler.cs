using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        foreach (var state in FindObjectsOfType<State>())
            state.CollisionCount++;
    }
}
