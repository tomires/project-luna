using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Teleporter")
            foreach (var state in FindObjectsOfType<State>())
                state.ChangeLevel();
        else
            foreach (var state in FindObjectsOfType<State>())
                state.TriggerCollision();
    }
}
