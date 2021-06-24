using UnityEngine;

public class SupervisorLights : MonoSingleton<SupervisorLights>
{
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
