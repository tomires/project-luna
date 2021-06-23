using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private State state;

    private void Awake()
    {
        state = FindObjectOfType<State>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(state)
            state.CollisionCount++;
    }
}
