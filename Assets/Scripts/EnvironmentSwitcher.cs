using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitcher : MonoSingleton<EnvironmentSwitcher>
{
    [SerializeField] private GameObject calibrationEnvironment;
    [SerializeField] private GameObject floorReference;
    [SerializeField] private GameObject playbackBackgroundPrefab;
    [SerializeField] private List<GameObject> environments;
    [SerializeField] private List<float> lightMultipliers;
    [SerializeField] public List<Light> lights;
    public List<GameObject> Environments => environments;

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
            if (!Initializer.Instance.supervisor)
                SetLights();
        }
        else if (currentEnvironment == environments.Count)
        {
            environments[currentEnvironment - 1].SetActive(false);
            FindObjectOfType<State>().EndExperiment();
        }
    }

    public void SwitchToPlaybackLayout()
    {
        var environmentSize = floorReference.transform.localScale + Constants.PlaybackEnvironmentSpacing * Vector3.one;
        var e = 0;

        foreach (var environment in environments)
        {
            environment.transform.SetParent(null);
            environment.transform.gameObject.SetActive(true);
            environment.transform.position =
                e % Constants.PlaybackEnvironmentsPerRow * environmentSize.z * Vector3.forward
                + Mathf.Floor(e / Constants.PlaybackEnvironmentsPerRow) * environmentSize.x * Vector3.right;

            var background = Instantiate(playbackBackgroundPrefab, environment.transform);
            playbackBackgroundPrefab.transform.localPosition = floorReference.transform.localPosition;
            background.transform.localScale = floorReference.transform.localScale;
            e++;
        }

        foreach (var child in GetComponentsInChildren<Transform>())
            child.gameObject.SetActive(false);
    }

    private void SetLights()
    {
        var multiplier = lightMultipliers[currentEnvironment];
        var offset = multiplier * Calibrator.Instance.LightIntensityVariance;
        var intensity = Calibrator.Instance.MinLightIntensity + offset;
        FindObjectOfType<State>().CurrentLuminance = intensity;

        foreach (var light in lights)
            light.intensity = intensity;
    }
}
