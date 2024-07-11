using UnityEngine;

/// <summary>
/// Detects collisions with objects tagged as "Carriable" and "Punchable".
/// Adds "Carriable" objects to the CharacterMovement's list of carried objects.
/// Punches "Punchable" objects, applies a force opposite to the player, makes it go ragdoll, and changes its tag to "Carriable" after 1 second.
/// </summary>
public class PlayerCollider : MonoBehaviour {
    [SerializeField] private CharacterMovement characterMovement;

    private void Start() {
        if (characterMovement == null) characterMovement = transform.GetComponent<CharacterMovement>();
    }

    /// <summary>
    /// Triggered when another collider enters the trigger collider of this object.
    /// Handles "Carriable" objects by adding them to the CharacterMovement's carried objects list.
    /// Punches "Punchable" objects by applying a force opposite to the player, making it go ragdoll, and changing its tag to "Carriable" after 1 second.
    /// </summary>
    /// <param name="other">The Collider of the other object that entered the trigger.</param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Carriable")) {
            if (other.attachedRigidbody != null) {
                characterMovement.AddCarriedObject(other.transform);
            }
        } else if (other.CompareTag("Punchable")) {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null) {
                // Calculate opposite force direction relative to the player
                Vector3 forceDirection = other.transform.position - transform.position;
                forceDirection.Normalize();

                // Apply force to the Punchable object
                rb.AddForce(forceDirection * 10f, ForceMode.Impulse);

                // Make the Punchable object go ragdoll (if it has a Ragdoll component, adjust accordingly)
                RagdollController ragdollController = other.GetComponent<RagdollController>();
                if (ragdollController != null) {
                    ragdollController.SetRagdollEnabled(true);
                }

                // Change the tag of the Punchable object to "Carriable" after 1 second
                StartCoroutine(ChangeTagDelayed(other.gameObject, "Carriable", 1f));
            }
        }
    }

    /// <summary>
    /// Changes the tag of the specified GameObject after a delay.
    /// </summary>
    /// <param name="gameObject">The GameObject whose tag should be changed.</param>
    /// <param name="newTag">The new tag to assign to the GameObject.</param>
    /// <param name="delay">The delay (in seconds) before changing the tag.</param>
    private System.Collections.IEnumerator ChangeTagDelayed(GameObject gameObject, string newTag, float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.tag = newTag;
    }
}
