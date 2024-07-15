using System.Collections;
using UnityEngine;

public class PlayerCollider : MonoBehaviour {
    [Header("References")]
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Animator animator;
    [SerializeField] private UIController uiController;

    [Header("Settings")]
    [SerializeField] private float punchForce = 10f;
    [SerializeField] private float changeTagTime = 2.0f;
    [SerializeField] private int sellValue = 100;

    [Header("Tags")]
    [SerializeField] private string punchableTag = "Punchable";
    [SerializeField] private string carriableTag = "Carriable";
    [SerializeField] private string dropTag = "Drop";
    [SerializeField] private string floorTag = "floor";

    private void Start() {
        if (characterMovement == null) {
            characterMovement = GetComponent<CharacterMovement>();
        }

        if (animator == null) {
            animator = characterMovement.PlayerAnimator;
        }

        if (uiController == null) {
            uiController = FindAnyObjectByType<UIController>();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag(floorTag)) return;

        GameObject parentObject = GetTopmostParentWithTag(other.gameObject, punchableTag, carriableTag);
        if (parentObject == null) return;

        Rigidbody parentRigidbody = parentObject.GetComponent<Rigidbody>();
        if (parentObject.CompareTag(carriableTag) && characterMovement.MaxStack > characterMovement.CarriedObjects.Count) {
            HandleCarriableObject(parentObject, parentRigidbody);
        } else if (parentObject.CompareTag(punchableTag)) {
            HandlePunchableObject(parentObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(dropTag) && characterMovement.CarriedObjects.Count > 0) {
            int moneyEarned = sellValue * characterMovement.CarriedObjects.Count;
            characterMovement.RemoveCarriedObjects();
            uiController.UpdateMoney(moneyEarned);
            characterMovement.UpdatePlayerColor();
        }
    }

    private void HandleCarriableObject(GameObject parentObject, Rigidbody parentRigidbody) {
        Animator parentAnimator = parentObject.GetComponentInChildren<Animator>();
        if (parentAnimator != null) parentAnimator.enabled = true;

        if (parentRigidbody == null) {
            parentRigidbody = parentObject.AddComponent<Rigidbody>();
        }

        ResetRigidbodyForces(parentObject);
        DisableAllColliders(parentObject);
        DisableHipsGameObject(parentObject);

        characterMovement.AddCarriedObject(parentRigidbody);
        parentRigidbody.useGravity = false;
    }

    private void HandlePunchableObject(GameObject punchableObject) {
        Rigidbody spineRb = GetRigidbodyByName(punchableObject, "mixamorig:Spine1");
        if (spineRb != null) {
            Animator punchableAnimator = punchableObject.GetComponentInChildren<Animator>();
            if (punchableAnimator != null) {
                punchableAnimator.enabled = false;
            }

            ApplyPunchForce(spineRb, punchableObject.transform.position - transform.position);

            StartCoroutine(ChangeTagDelayed(punchableObject, carriableTag, changeTagTime));
        }

        // Trigger punch animation
        animator.SetTrigger("Punch");
    }

    private void ApplyPunchForce(Rigidbody spineRb, Vector3 forceDirection) {
        forceDirection.Normalize();
        Vector3 punchForceDirection = -forceDirection * punchForce + Vector3.up;
        spineRb.AddForce(punchForceDirection, ForceMode.Impulse);

        Quaternion lookRotation = Quaternion.LookRotation(-forceDirection);
        spineRb.transform.rotation = lookRotation;
    }

    private void ResetRigidbodyForces(GameObject obj) {
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>()) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }
    }

    private void DisableAllColliders(GameObject obj) {
        foreach (var col in obj.GetComponentsInChildren<Collider>()) {
            col.enabled = false;
        }
    }

    private void DisableHipsGameObject(GameObject obj) {
        Transform hipsTransform = obj.transform.Find("mixamorig:Hips");
        if (hipsTransform != null) {
            hipsTransform.gameObject.SetActive(false);
        }
    }

    private IEnumerator ChangeTagDelayed(GameObject gameObject, string newTag, float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.tag = newTag;
    }

    private GameObject GetTopmostParentWithTag(GameObject obj, params string[] tags) {
        Transform current = obj.transform;
        Transform topmostParent = current;

        while (current.parent != null) {
            current = current.parent;
            if (System.Array.Exists(tags, tag => current.CompareTag(tag))) {
                topmostParent = current;
            }
        }

        return System.Array.Exists(tags, tag => topmostParent.CompareTag(tag)) ? topmostParent.gameObject : null;
    }

    private Rigidbody GetRigidbodyByName(GameObject obj, string name) {
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>()) {
            if (rb.name == name) {
                return rb;
            }
        }
        return null;
    }
}
