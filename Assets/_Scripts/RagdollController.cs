using UnityEngine;

/// <summary>
/// Controls the ragdoll state of a character.
/// </summary>
public class RagdollController : MonoBehaviour {
    // Array of Rigidbodies to control
    [SerializeField] private Rigidbody[] rigidbodies;

    // Array of Colliders to control
    [SerializeField] private Collider[] colliders;

    private void Awake() {
        // Disable the rigidbodies and colliders initially
        SetRagdollEnabled(false);
    }

    /// <summary>
    /// Enables or disables the ragdoll effect.
    /// </summary>
    /// <param name="ragdollEnabled">True to enable ragdoll, false to disable.</param>
    public void SetRagdollEnabled(bool ragdollEnabled) {

    }
}
