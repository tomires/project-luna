using UnityEngine;

public class ArrowManager : MonoSingleton<ArrowManager>
{
    public void ChangeArrowDirection()
    {
        foreach (var arrow in GetComponentsInChildren<SpriteRenderer>())
            arrow.flipY = !arrow.flipY;
    }
}
