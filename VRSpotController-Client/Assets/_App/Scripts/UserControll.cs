using System;
using UnityEngine;
using UnityEngine.XR;

public class UserControll : MonoBehaviour
{
    private UserInput input;
    private SpotServer server;

    private void Start()
    {
        input = new UserInput();
        input.Controll.Enable();
        
        server = new SpotServer();
    }

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 move = input.Controll.Move.ReadValue<Vector2>();
        server.Move(move);
    }
}
