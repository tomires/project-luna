using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Calibrator : MonoBehaviour
{
    private InputDevice rController;
    private InputDevice lController;
    private float positioningSpeed = 0.01f;
    private float rotationSpeed = 0.3f;
    private bool previouslyPressed = false;

    void Start()
    {
        rController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        lController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        if (lController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lJoystick)) {
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

        if(rController.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed))
        {
            if (pressed && !previouslyPressed && FindObjectOfType<State>())
                FindObjectOfType<State>().ChangeLevel();
            previouslyPressed = pressed;
        }
    }
}
