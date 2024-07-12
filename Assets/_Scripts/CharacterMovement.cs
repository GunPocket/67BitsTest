using System.Collections.Generic;
using UnityEngine;

public class Particle {
    public Vector3 position;
    public Vector3 previousPosition;

    public Particle(Vector3 initialPosition) {
        position = initialPosition;
        previousPosition = initialPosition;
    }
}

public class CharacterMovement : MonoBehaviour {
    private Vector3 playerVelocity;
    [SerializeField] private float playerSpeed = 2.0f;

    [Header("Needed references")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("List of carried objects")]
    [SerializeField] private List<Particle> particles = new();
    [SerializeField] private List<Rigidbody> carriedObjects = new();

    [Header("Stacking settings")]
    [SerializeField] private float segmentDistance = 1.0f;
    [SerializeField] private float upwardForce = 10.0f;
    [SerializeField][Range(0, 1)] private float upwardForceDamping = 0.1f;

    private void Start() {
        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (rb == null) rb = GetComponent<Rigidbody>();

        InitializeParticles();
    }

    private void FixedUpdate() {
        Move(inputHandler.InputDirection);
        UpdateParticles();
        ApplyUpwardForce();
        PositionCarriedObjects();
        RotateCarriedObjects();
    }

    private void InitializeParticles() {
        Vector3 startPosition = transform.position + Vector3.up * segmentDistance;
        particles.Clear();
        particles.Add(new Particle(startPosition));
        for (int i = 0; i < carriedObjects.Count; i++) {
            particles.Add(new Particle(startPosition + (i + 1) * segmentDistance * Vector3.up));
        }
    }

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

        particles[0].position = transform.position + Vector3.up * segmentDistance;
    }

    private void UpdateParticles() {
        //Using Verlet Integration

        for (int i = 1; i < particles.Count; i++) {
            Particle current = particles[i];
            Particle previous = particles[i - 1];

            Vector3 direction = (current.position - previous.position).normalized;
            Vector3 targetPosition = previous.position + direction * segmentDistance;

            Vector3 newPosition = targetPosition;
            current.previousPosition = current.position;
            current.position = newPosition;
        }
    }

    private void ApplyUpwardForce() {
        for (int i = 1; i < particles.Count; i++) {
            float influenceFactor = 1.0f - (i * upwardForceDamping);
            particles[i].position += influenceFactor * Time.fixedDeltaTime * upwardForce * Vector3.up;
        }
    }

    private void PositionCarriedObjects() {
        for (int i = 0; i < carriedObjects.Count; i++) {
            if (i + 1 < particles.Count) {
                carriedObjects[i].MovePosition(particles[i + 1].position);
            }
        }
    }

    private void RotateCarriedObjects() {
        Quaternion playerRotation = rb.rotation;

        for (int i = 0; i < carriedObjects.Count; i++) {
            carriedObjects[i].MoveRotation(playerRotation * Quaternion.Euler(90, 0, -90));
        }
    }

    public void AddCarriedObject(Rigidbody newObject) {
        carriedObjects.Add(newObject);

        InitializeParticles();
    }

    public void RemoveCarriedObject(Rigidbody oldObject) {
        int index = carriedObjects.IndexOf(oldObject);
        if (index >= 0) {
            carriedObjects.RemoveAt(index);
            if (index + 1 < particles.Count) {
                particles.RemoveAt(index + 1); // Remove corresponding particle
            }
        }
        oldObject.transform.SetParent(null);
    }
}
