using UnityEngine;

public class SupervisorLights : MonoSingleton<SupervisorLights>
{
    public void Show()
    {
        foreach(var light in GetComponentsInChildren<Light>())
            light.enabled = true;
    }
}
