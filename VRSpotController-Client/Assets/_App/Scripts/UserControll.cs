using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UserControll : MonoBehaviour
{
    private UserInput input;
    
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;

    private void Start()
    {
        input = new UserInput();
        input.Controll.Enable();
        input.Status.Enable();

        input.Controll.Stand.performed += OnStandPressed;
        input.Controll.Power.performed += OnPowerOnPressed;
    }

    private void Update()
    {
        UpdateControllers();
        HandleMovement();
        HandlePitch();
        HandleGrip();
        HandleArm();
    }

    void UpdateControllers()
    {
        Vector3 leftPos = input.Status.LeftControllerPos.ReadValue<Vector3>();
        leftController.localPosition = leftPos;
        Quaternion leftRot = input.Status.LeftControllerRot.ReadValue<Quaternion>();
        leftController.localRotation = leftRot;
        Vector3 rightPos = input.Status.RightControllerPos.ReadValue<Vector3>();
        rightController.localPosition = rightPos;
        Quaternion rightRot = input.Status.RightControllerRot.ReadValue<Quaternion>();
        rightController.localRotation = rightRot;
    }

    void HandleArm()
    {
        var pos = input.Controll.ArmPos.ReadValue<Vector3>();
        Spot.instance.ArmPos(pos);
    }

    void HandleMovement()
    {
        var dir = input.Controll.Move.ReadValue<Vector2>();
        Spot.instance.Move(dir);
        var yaw = input.Controll.Yaw.ReadValue<float>();
        Spot.instance.Yaw(yaw);
    }

    void HandlePitch()
    {
        var pitch = input.Controll.Pitch.ReadValue<float>();
        Spot.instance.Pitch(pitch);
    }

    void HandleGrip()
    {
        var grip = input.Controll.Grip.ReadValue<float>();
        grip = math.clamp(grip, 0f, 1f);
        Spot.instance.Grip(grip);
    }

    void OnStandPressed(InputAction.CallbackContext context)
    {
        Spot.instance.Stand();
    }

    void OnPowerOnPressed(InputAction.CallbackContext context)
    {
        Spot.instance.PowerOn();
    }
}
