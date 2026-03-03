using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class UserControll : MonoBehaviour
{
    private const int SENDING_FREQUENCY = 10;
    private const int SENDING_MS = 1 /  SENDING_FREQUENCY * 1000;
    
    private UserInput input;
    private SpotServer server;

    private Vector2 lastReadedVelocity = Vector2.zero;
    private Vector2 lastSendedVelocity = Vector2.zero;
    private DateTime lastSendedTime = DateTime.Now;

    private void Start()
    {
        input = new UserInput();
        input.Controll.Enable();

        input.Controll.Stand.performed += OnStandPressed;
        input.Controll.Power.performed += OnPowerOnPressed;
        
        server = new SpotServer();
    }

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        lastReadedVelocity = input.Controll.Move.ReadValue<Vector2>();
        
        if(lastSendedVelocity == lastReadedVelocity)return;
        if ((DateTime.Now - lastSendedTime).TotalMilliseconds < SENDING_MS)return;
        
        server.Move(lastReadedVelocity);
        lastSendedVelocity = lastReadedVelocity;
        lastSendedTime = DateTime.Now;
    }

    void OnStandPressed(InputAction.CallbackContext context)
    {
        server.SetStanding(!server.isStanding);
    }

    void OnPowerOnPressed(InputAction.CallbackContext context)
    {
        server.SetPowerOn(!server.isPoweredOn);
    }
}
