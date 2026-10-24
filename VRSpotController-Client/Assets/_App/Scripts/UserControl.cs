using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserControl : MonoBehaviour
{
    private UserInput input;
    
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;

    private void Start()
    {
        input = new UserInput();
        input.Control.Enable();
        input.Status.Enable();

        input.Control.Stand.performed += OnStandPressed;
        input.Control.Power.performed += OnPowerOnPressed;
        input.Control.ArmControl.started += OnBeginArmControl;
        input.Control.ArmControl.canceled += OnEndArmControl;
    }

    private void Update()
    {
        UpdateControllers();
        HandleMovement();
        HandleRotate();
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
        Spot.instance.UserHandTransform(rightController.position, rightController.rotation, rightController.localPosition, rightController.localRotation);
    }

    void HandleMovement()
    {
        var dir = input.Control.Move.ReadValue<Vector2>();
        Spot.instance.Move(dir);
    }

    void HandleRotate()
    {
        var rotate = input.Control.Rotate.ReadValue<Vector2>();
        if (math.abs(rotate.x) > math.abs(rotate.y))
        {
            rotate.y = 0f;
        }
        else
        {
            rotate.x = 0f;
        }

        Spot.instance.Rotate(rotate);
    }

    void HandleGrip()
    {
        var grip = input.Control.Grip.ReadValue<float>();
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

    void OnBeginArmControl(InputAction.CallbackContext context)
    {
        Spot.instance.SetArmControl(true);
    }
    
    void OnEndArmControl(InputAction.CallbackContext context)
    {
        Spot.instance.SetArmControl(false);
    }
}
