using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input using the PlayerControlls InputSystem and updates InputDirection based on movement actions.
/// </summary>
public class PlayerInputHandler : MonoBehaviour {
    private PlayerControlls controller;
    public Vector2 InputDirection { get; private set; }

    private void Awake() {
        controller = new PlayerControlls();
    }

    /// <summary>
    /// Enables the PlayerControlls and subscribes to movement input events when the object becomes enabled.
    /// </summary>
    private void OnEnable() {
        controller.Player.Enable();
        controller.Player.Move.performed += OnMovePerformed;
        controller.Player.Move.canceled += OnMoveCanceled;
    }

    /// <summary>
    /// Disables the PlayerControlls and unsubscribes from movement input events when the object becomes disabled.
    /// </summary>
    private void OnDisable() {
        controller.Player.Disable();
        controller.Player.Move.performed -= OnMovePerformed;
        controller.Player.Move.canceled -= OnMoveCanceled;
    }

    /// <summary>
    /// Callback method called when a movement input action is performed, updating the InputDirection.
    /// </summary>
    /// <param name="context">Input action callback context containing the input data.</param>
    private void OnMovePerformed(InputAction.CallbackContext context) {
        InputDirection = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback method called when a movement input action is canceled, resetting the InputDirection to zero.
    /// </summary>
    /// <param name="context">Input action callback context containing the input data.</param>
    private void OnMoveCanceled(InputAction.CallbackContext context) {
        InputDirection = Vector2.zero;
    }
}
