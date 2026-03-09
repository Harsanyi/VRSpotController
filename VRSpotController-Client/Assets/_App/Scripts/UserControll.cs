using System;
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
    }
    
    void HandleMovement()
    {
        var dir = input.Controll.Move.ReadValue<Vector2>();
        Spot.instance.Move(dir);
        var yaw = input.Controll.Yaw.ReadValue<float>();
        Spot.instance.Yaw(yaw);
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
