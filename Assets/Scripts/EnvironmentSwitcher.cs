using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitcher : MonoSingleton<EnvironmentSwitcher>
{
    [SerializeField] private GameObject calibrationEnvironment;
    [SerializeField] private List<GameObject> environments;
    int currentEnvironment = -1;

    public void MoveToNextEnvironment()
    {
        currentEnvironment++;

        if(currentEnvironment == 0)
        {
            calibrationEnvironment.SetActive(false);
            environments[0].SetActive(true);
        }
        else if(currentEnvironment < environments.Count)
        {
            environments[currentEnvironment - 1].SetActive(false);
            environments[currentEnvironment].SetActive(true);
        }
        else
        {
            environments[currentEnvironment - 1].SetActive(false);
            FindObjectOfType<State>().EndExperiment();
        }
    }
}
