using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    private PlayerControlls controller;

    public Vector2 InputDirection { get; private set; }

    private void Awake() {
        controller = new PlayerControlls();
    }

    private void OnEnable() {
        controller.Player.Enable();
        controller.Player.Move.performed += OnMovePerformed;
        controller.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable() {
        controller.Player.Disable();
        controller.Player.Move.performed -= OnMovePerformed;
        controller.Player.Move.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context) {
        InputDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) {
        InputDirection = Vector2.zero;
    }
}
