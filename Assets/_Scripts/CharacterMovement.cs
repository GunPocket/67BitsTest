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
    [Header("Player Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private Material playerMaterial;
    public Animator PlayerAnimator;

    [Header("Needed References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("List of Carried Objects")]
    [SerializeField] private List<Particle> particles = new();
    public List<Rigidbody> CarriedObjects = new();

    [Header("Stacking Settings")]
    [SerializeField] private float segmentDistance = 1.0f;
    [SerializeField] private float upwardForce = 10.0f;
    [SerializeField][Range(0, 1)] private float upwardForceDamping = 0.1f;
    [SerializeField] private int maxStack = 1;

    private Vector3 playerVelocity;

    public int MaxStack {
        get => maxStack;
        set {
            maxStack = value;
            UpdatePlayerColor();
        }
    }

    private void Start() {
        inputHandler = inputHandler != null ? inputHandler : GetComponent<PlayerInputHandler>();
        mainCamera = mainCamera != null ? mainCamera : Camera.main;
        rb = rb != null ? rb : GetComponent<Rigidbody>();
        PlayerAnimator = PlayerAnimator != null ? PlayerAnimator : GetComponent<Animator>();

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
        for (int i = 0; i < CarriedObjects.Count; i++) {
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
            PlayerAnimator.SetBool("Idle", false);
        } else {
            PlayerAnimator.SetBool("Idle", true);
        }

        if (particles.Count > 0) {
            particles[0].position = transform.position + Vector3.up * segmentDistance;
        }
    }

    private void UpdateParticles() {
        for (int i = 1; i < particles.Count; i++) {
            Particle current = particles[i];
            Particle previous = particles[i - 1];

            Vector3 direction = (current.position - previous.position).normalized;
            Vector3 targetPosition = previous.position + direction * segmentDistance;

            current.previousPosition = current.position;
            current.position = targetPosition;
        }
    }

    private void ApplyUpwardForce() {
        for (int i = 1; i < particles.Count; i++) {
            float influenceFactor = 1.0f - (i * upwardForceDamping);
            particles[i].position += influenceFactor * Time.fixedDeltaTime * upwardForce * Vector3.up;
        }
    }

    private void PositionCarriedObjects() {
        for (int i = 0; i < CarriedObjects.Count; i++) {
            if (i + 1 < particles.Count && CarriedObjects[i] != null) {
                CarriedObjects[i].MovePosition(particles[i + 1].position);
            }
        }
    }

    private void RotateCarriedObjects() {
        Quaternion playerRotation = rb.rotation;

        for (int i = 0; i < CarriedObjects.Count; i++) {
            if (CarriedObjects[i] != null) {
                CarriedObjects[i].MoveRotation(playerRotation * Quaternion.Euler(90, 0, -90));
            }
        }
    }

    public void AddCarriedObject(Rigidbody newObject) {
        CarriedObjects.Add(newObject);
        InitializeParticles();
    }

    public void RemoveCarriedObjects() {
        foreach (Rigidbody co in CarriedObjects) {
            if (co != null) {
                Destroy(co.gameObject);
            }
        }

        CarriedObjects.Clear();
        particles.Clear();
        InitializeParticles();
    }

    public void UpdatePlayerColor() {
        Color newColor = CalculateColor(MaxStack);
        playerMaterial.color = newColor;
    }

    private Color CalculateColor(int maxStack) {
        float hue = (maxStack % 360) / 15f;
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    public void IncreaseMaxStack() {
        MaxStack++;
    }
}
