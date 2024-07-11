using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of a character and manages objects carried by the character.
/// </summary>
public class CharacterMovement : MonoBehaviour {
    private Vector3 playerVelocity;
    private readonly float playerSpeed = 2.0f;

    [Header("Needed references")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("List of carried objects")]
    [SerializeField] private List<Transform> carriedObjects = new();

    [Header("Inertia for carried objects")]
    [SerializeField][Range(0, 3)] private float inertiaPositionFactor = 0.5f;
    [SerializeField][Range(0, 3)] private float inertiaRotationFactor = 0.5f;

    private void Start() {
        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();

        if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();

        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Move(inputHandler.InputDirection);
    }

    /// <summary>
    /// Moves the character based on the input direction and updates carried objects' positions and rotations.
    /// </summary>
    /// <param name="direction">Input direction from the player.</param>
    private void Move(Vector2 direction) {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        playerVelocity = (forward * direction.y + right * direction.x) * playerSpeed;
        rb.velocity = playerVelocity;

        if (direction != Vector2.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(playerVelocity);
            rb.MoveRotation(targetRotation);
        }

        PositionCarriedObjects();
    }

    /// <summary>
    /// Positions and rotates carried objects relative to the character's position and rotation.
    /// </summary>
    private void PositionCarriedObjects() {
        if (carriedObjects.Count == 0) return;

        Vector3 targetPosition = transform.position + transform.forward + Vector3.up;
        Quaternion targetRotation = transform.rotation;

        carriedObjects[0].SetPositionAndRotation(targetPosition, targetRotation);

        for (int i = 1; i < carriedObjects.Count; i++) {
            Transform carriedTransform = carriedObjects[i];
            targetPosition = carriedObjects[i - 1].position + Vector3.up;
            targetRotation = carriedObjects[i - 1].rotation;
            carriedTransform.SetPositionAndRotation(
                Vector3.Lerp(carriedTransform.position, targetPosition, Time.fixedDeltaTime * 20 * inertiaPositionFactor),
                Quaternion.Lerp(carriedTransform.rotation, targetRotation, Time.fixedDeltaTime * 50 * inertiaRotationFactor));
        }
    }

    /// <summary>
    /// Adds a new object to be carried by the character.
    /// </summary>
    /// <param name="newObject">Transform of the new object to be added.</param>
    public void AddCarriedObject(Transform newObject) {
        newObject.rotation = Quaternion.identity;
        carriedObjects.Add(newObject);
        Vector3 targetPosition = transform.position + transform.forward * 1 + Vector3.up * (1 + carriedObjects.Count * 0.5f);
        newObject.position = Vector3.Lerp(newObject.position, targetPosition, Time.deltaTime * 10);
    }

    /// <summary>
    /// Removes a carried object from the list of carried objects.
    /// </summary>
    /// <param name="oldObject">Transform of the object to be removed.</param>
    public void RemoveCarriedObject(Transform oldObject) {
        carriedObjects.Remove(oldObject);
        oldObject.SetParent(null);
    }
}
