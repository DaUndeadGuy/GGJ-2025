using UnityEngine;

public class PlayerInteracter : MonoBehaviour
{
    // Tag for interactable objects
    [SerializeField] private string interactableTag = "Interactable";
    // Reference to the current interactable object
    private GameObject currentInteractable;

    // Offset and size for the collider box
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 boxOffset = new Vector3(1f, 0f, 0f);
    private Vector3 baseOffset;

    void Update()
    {
        // Check if the player presses E and there's an interactable object
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Interact();
        }

        baseOffset = boxOffset;
    }

    void FixedUpdate()
    {

        if (PlayerController.Instance.lastMoveDirection.x < 0) // Moving left
        {
            boxOffset = new Vector3(-0.360000014f, 0, 0);
            boxSize = new Vector3(0.379999995f, 0.340000004f, 0.189999998f);

        }
        else if (PlayerController.Instance.lastMoveDirection.x > 0) // Moving right
        {
            boxOffset = new Vector3(0.379999995f, 0, 0);
            boxSize = new Vector3(0.379999995f, 0.340000004f, 0.189999998f);
        }
        else if (PlayerController.Instance.lastMoveDirection.z > 0) // Moving up
        {
            boxOffset = new Vector3(0, 0, 0.270000011f);
            boxSize = new Vector3(0.140000001f, 0.340000004f, 0.409999996f);
        }
        else if (PlayerController.Instance.lastMoveDirection.z < 0) // Moving down
        {
            boxOffset = new Vector3(0, 0, -0.300000012f);
            boxSize = new Vector3(0.140000001f, 0.340000004f, 0.409999996f);

        }

        Debug.Log(boxOffset);
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

        if (currentInteractable.GetComponent<CutsceneTrigger>() != null)
        {
            CutsceneTrigger cutsceneT = currentInteractable.GetComponent<CutsceneTrigger>();

            // Focus on the object using the CutsceneManager
            CutsceneManager.Instance.FocusOnObject(
                cutsceneT.FocusTarget,       // The target Transform to focus on
                cutsceneT.OnCutsceneEnd     // Optional UnityEvent to trigger after the cutscene
            );
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize the box collider in the editor
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformDirection(boxOffset), transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
