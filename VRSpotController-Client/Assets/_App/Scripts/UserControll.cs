using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UserControll : MonoBehaviour
{
    private UserInput input;

    private void Start()
    {
        input = new UserInput();
        input.Controll.Enable();

        input.Controll.Stand.performed += OnStandPressed;
        input.Controll.Power.performed += OnPowerOnPressed;
    }

    private void Update()
    {
        HandleMovement();
        HandlePitch();
        HandleGrip();
        HandleArm();
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
