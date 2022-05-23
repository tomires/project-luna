using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System;

public class Calibrator : MonoSingleton<Calibrator>
{
    public float MinLightIntensity
    {
        get => minLightIntensity;

        set
        {
            if (value < 0f) return;
            minLightIntensity = value;
            PropagateLightIntensity(minLightIntensity);
        }
    }

    public float MaxLightIntensity
    {
        get => maxLightIntensity;

        set
        {
            if (value < minLightIntensity) return;
            maxLightIntensity = value;
            PropagateLightIntensity(maxLightIntensity);
        }
    }

    public float LightIntensityVariance
        => MaxLightIntensity - MinLightIntensity;

    [SerializeField] private Text lightSettingText;

    private InputDevice rController;
    private InputDevice lController;
    private float positioningSpeed = 0.025f;
    private float rotationSpeed = 0.3f;
    private float lightIntensityChangeSpeed = 0.025f;
    private bool previouslyPressed = false;
    private CalibrationState calibrationState = CalibrationState.EnvironmentOffset;

    private enum CalibrationState
    {
        EnvironmentOffset, MinLightIntensity, MaxLightIntensity, Done
    }

    private float minLightIntensity = 0.4f;
    private float maxLightIntensity = 2f;

    void Start()
    {
        rController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        lController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        switch(calibrationState)
        {
            case CalibrationState.EnvironmentOffset:
                ControlEnvironmentOffset();
                break;
            case CalibrationState.MinLightIntensity:
            case CalibrationState.MaxLightIntensity:
                ControlLightIntensity();
                break;
        }

        if (rController.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed))
        {
            if (pressed && !previouslyPressed)
                AdvanceState();
            previouslyPressed = pressed;
        }
    }

    private void ControlEnvironmentOffset()
    {
        if (lController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lJoystick))
        {
            if (Mathf.Abs(lJoystick.x) > 0.5f)
                transform.position -= (positioningSpeed * lJoystick.x) * Vector3.right;
            if (Mathf.Abs(lJoystick.y) > 0.5f)
                transform.position -= (positioningSpeed * lJoystick.y) * Vector3.forward;
        }

        if (rController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rJoystick))
        {
            if (Mathf.Abs(rJoystick.x) > 0.5f)
                transform.rotation *= Quaternion.Euler(rotationSpeed * rJoystick.x * Vector3.up);
        }
    }

    private void ControlLightIntensity()
    {
        if (lController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lJoystick))
        {
            if (Mathf.Abs(lJoystick.y) > 0.5f)
            {
                var offset = (lightIntensityChangeSpeed * lJoystick.y);
                if (lController.TryGetFeatureValue(CommonUsages.gripButton, out bool pressed))
                    if (pressed)
                        offset *= 10f;

                AudioPlayer.Instance.PlaySound(offset > 0 ? Constants.Sounds.Up : Constants.Sounds.Down);

                switch(calibrationState)
                {
                    case CalibrationState.MinLightIntensity:
                        MinLightIntensity += offset;
                        break;
                    case CalibrationState.MaxLightIntensity:
                        MaxLightIntensity += offset;
                        break;
                }
            }
        }
    }

    private void PropagateLightIntensity(float intensity)
    {
        foreach (var light in FindObjectsOfType<Light>())
            light.intensity = intensity;

        lightSettingText.text = 
            (calibrationState == CalibrationState.MinLightIntensity
            ? "Lmin " : "Lmax ") + string.Format("{0:0.00}", intensity);
    }

    private void AdvanceState()
    {
        calibrationState = calibrationState switch
        {
            CalibrationState.EnvironmentOffset => CalibrationState.MinLightIntensity,
            CalibrationState.MinLightIntensity => CalibrationState.MaxLightIntensity,
            _ => CalibrationState.Done
        };

        switch(calibrationState)
        {
            case CalibrationState.MinLightIntensity:
                MinLightIntensity = MinLightIntensity;
                break;
            case CalibrationState.MaxLightIntensity:
                FindObjectOfType<State>().LuminanceLowerBound = MinLightIntensity;
                MaxLightIntensity = MinLightIntensity;
                break;
            case CalibrationState.Done:
                FindObjectOfType<State>().LuminanceUpperBound = MaxLightIntensity;
                if (FindObjectOfType<State>())
                    FindObjectOfType<State>().ChangeLevel();
                else
                    lightSettingText.text = "Connect phone!";
                break;
        }
    }
}
