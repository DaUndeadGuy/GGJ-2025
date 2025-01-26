using UnityEngine;

public class BubbleCollisionChecker : MonoBehaviour
{
    public RectTransform rectTransform; // RectTransform of the current UI element
    private DraggableBubble draggableBubble; // Reference to DraggableBubble script
    private DialogueTrigger dialogueTrigger; // Reference to DialogueTrigger if attached

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

    private void Update()
    {
        CheckForUIOverlaps();
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
                HandleCollision(otherBubble.gameObject);
            }
        }
    }

    private void HandleCollision(GameObject otherBubble)
    {
        // Get the DraggableBubble component of the other bubble
        DraggableBubble otherDraggableBubble = otherBubble.GetComponent<DraggableBubble>();
        if (otherDraggableBubble == null)
        {
            Debug.LogWarning($"[BubbleCollisionChecker] Other bubble {otherBubble.name} is missing DraggableBubble component.");
            return;
        }

        // Check if bubble types are the same
        if (draggableBubble.bubbleType == otherDraggableBubble.bubbleType)
        {
            Debug.Log($"[BubbleCollisionChecker] Collision detected between {gameObject.name} and {otherBubble.name}. Both bubbles have the same type ({draggableBubble.bubbleType}).");

            // Hide both bubbles by adjusting their alpha
            HideBubble(gameObject);
            HideBubble(otherBubble);

            // Trigger dialogue if attached
            if (dialogueTrigger != null)
            {
                Debug.Log($"[BubbleCollisionChecker] Triggering dialogue for {gameObject.name}.");
                dialogueTrigger.TriggerDialogue();
            }
        }
        else
        {
            Debug.Log($"[BubbleCollisionChecker] Collision detected between {gameObject.name} and {otherBubble.name}. Bubble types are different ({draggableBubble.bubbleType} vs {otherDraggableBubble.bubbleType}).");
        }
    }

    private void HideBubble(GameObject bubble)
    {
        // Use CanvasGroup to adjust visibility
        CanvasGroup canvasGroup = bubble.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Make invisible
            canvasGroup.interactable = false; // Disable interaction
            canvasGroup.blocksRaycasts = false; // Ignore raycasts
        }
        else
        {
            Debug.LogWarning($"[BubbleCollisionChecker] No CanvasGroup found on {bubble.name}. Cannot adjust alpha.");
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
