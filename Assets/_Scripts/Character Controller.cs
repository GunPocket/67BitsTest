using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour {
    private PlayerControlls controller;
    private Vector3 playerVelocity;
    private float playerSpeed = 2.0f;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera mainCamera;

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
        Move(context.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) {
        Move(Vector2.zero);
    }

    private void Move(Vector2 direction) {
        // Convert the 2D input direction to 3D direction based on isometric camera
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        // Project the camera forward and right direction to the XZ plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        playerVelocity = (forward * direction.y + right * direction.x) * playerSpeed;
        rb.velocity = playerVelocity;

        // Rotate character to face the direction they're moving
        if (direction != Vector2.zero) {
            float angle = Mathf.Atan2(playerVelocity.x, playerVelocity.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        // Play running animation
        if (direction != Vector2.zero && playerVelocity.magnitude > 0.5f) {
            // Play running animation
        }

        // Stop running animation when stopped
        if (direction == Vector2.zero) {
            // Stop running animation
        }

        // Play idle animation when idle
        if (direction == Vector2.zero && playerVelocity.magnitude < 0.1f) {
            // Play idle animation
        }
    }
}
