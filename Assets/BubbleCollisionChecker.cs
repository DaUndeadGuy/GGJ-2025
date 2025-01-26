using UnityEngine;

public class BubbleCollisionChecker : MonoBehaviour
{
    public RectTransform rectTransform; // RectTransform of the current UI element
    private DraggableBubble draggableBubble; // Reference to DraggableBubble script
    private DialogueTrigger dialogueTrigger; // Reference to DialogueTrigger if attached
    private bool wasMovementEnabled; // Tracks previous movement state

    private void Awake()
    {
        // Ensure the RectTransform is assigned
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // Get the DraggableBubble component
        draggableBubble = GetComponent<DraggableBubble>();
        if (draggableBubble == null)
        {
            Debug.LogError($"[BubbleCollisionChecker] No DraggableBubble component found on {gameObject.name}.");
        }

        // Get the DialogueTrigger component (if attached)
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void Start()
    {
        // Initialize movement state
        wasMovementEnabled = PlayerController.Instance?.CanMove ?? true;
    }

    private void Update()
    {
        CheckForUIOverlaps();
        CheckMovementState();
    }

    private void CheckForUIOverlaps()
    {
        if (draggableBubble == null) return;

        // Get all other DraggableBubble objects in the scene
        DraggableBubble[] allBubbles = FindObjectsOfType<DraggableBubble>();

        foreach (var otherBubble in allBubbles)
        {
            // Skip self
            if (otherBubble.gameObject == gameObject) continue;

            // Check for RectTransform overlaps
            if (RectOverlaps(rectTransform, otherBubble.GetComponent<RectTransform>()))
            {
                // Check for dialogue trigger on either bubble
                DialogueTrigger otherDialogueTrigger = otherBubble.GetComponent<DialogueTrigger>();

                if (dialogueTrigger != null)
                {
                    Debug.Log($"[BubbleCollisionChecker] Triggering dialogue for {gameObject.name}.");
                    dialogueTrigger.TriggerDialogue();
                }
                else if (otherDialogueTrigger != null)
                {
                    Debug.Log($"[BubbleCollisionChecker] Triggering dialogue for {otherBubble.name}.");
                    otherDialogueTrigger.TriggerDialogue();
                }
                else
                {
                    Debug.Log($"[BubbleCollisionChecker] Overlap detected, but no dialogue triggers found for {gameObject.name} or {otherBubble.name}.");
                }
            }
        }
    }

    private void CheckMovementState()
    {
        // Check if movement has been re-enabled
        if (PlayerController.Instance != null)
        {
            bool isMovementEnabled = PlayerController.Instance.CanMove;

            if (isMovementEnabled && !wasMovementEnabled)
            {
                // Movement was just re-enabled, toggle bubbles off
                GameFlowManager.Instance.ToggleThoughtBubbles();
                Debug.Log("[BubbleCollisionChecker] Movement re-enabled. Toggling bubbles off.");
            }

            // Update the movement state tracker
            wasMovementEnabled = isMovementEnabled;
        }
    }

    private bool RectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        if (rect1 == null || rect2 == null) return false;

        // Get world corners of both RectTransforms
        Rect rect1Bounds = GetWorldRect(rect1);
        Rect rect2Bounds = GetWorldRect(rect2);

        // Check for overlaps
        return rect1Bounds.Overlaps(rect2Bounds);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
}
