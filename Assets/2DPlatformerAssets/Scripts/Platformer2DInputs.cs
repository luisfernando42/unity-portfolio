using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platformer2DInputs : MonoBehaviour
{
    public event EventHandler OnJumpAction;

    private Platformer2DActions platformer2DActions;

    private void Awake()
    {
        platformer2DActions = new Platformer2DActions();
        platformer2DActions.Enable();
        platformer2DActions.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector2Normalized()
    {
        Vector2 inputVector = platformer2DActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
