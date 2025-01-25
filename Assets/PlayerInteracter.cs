using UnityEngine;

public class PlayerInteracter : MonoBehaviour
{
    // Tag for interactable objects
    [SerializeField] private string interactableTag = "Interactable";
    [SerializeField] private SpriteRenderer playerSprite;
    // Reference to the current interactable object
    private GameObject currentInteractable;

    // Offset and size for the collider box
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 boxOffset = new Vector3(1f, 0f, 0f);

    public bool IsFacingLeft => playerSprite.flipX;

    void Update()
    {
        // Check if the player presses E and there's an interactable object
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Interact();
        }
    }

    void FixedUpdate()
    {

        if (IsFacingLeft)
        {
            boxOffset = new Vector3(-Mathf.Abs(boxOffset.x), boxOffset.y, boxOffset.z);
        }
        else
        {
            boxOffset = new Vector3(Mathf.Abs(boxOffset.x), boxOffset.y, boxOffset.z);
        }

        // Perform a box overlap to detect objects with the "Interactable" tag
        Collider[] hits = Physics.OverlapBox(transform.position + transform.TransformDirection(boxOffset), boxSize / 2, Quaternion.identity);

        currentInteractable = null; // Reset the interactable reference

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(interactableTag))
            {
                currentInteractable = hit.gameObject;
                break;
            }
        }
    }

    private void Interact()
    {
        // Interaction logic
        Debug.Log($"Interacted with: {currentInteractable.name}");
        if (currentInteractable.GetComponent<DialogueTrigger>() != null)
        {
            DialogueTrigger dialogueT = currentInteractable.GetComponent<DialogueTrigger>();
            dialogueT.TriggerDialogue();
        }
        // Example: Add specific behaviors here if needed
        // E.g., opening a door, picking up an item, etc.
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the box collider in the editor
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformDirection(boxOffset), transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
