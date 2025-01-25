using UnityEngine;

public class BubbleCollisionChecker : MonoBehaviour
{
    public RectTransform rectTransform; // RectTransform of the current UI element
    private DraggableBubble draggableBubble; // Reference to DraggableBubble script

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
                if (otherBubble.bubbleType == draggableBubble.bubbleType)
                {
                    Debug.Log($"[BubbleCollisionChecker] {gameObject.name} overlapped with {otherBubble.name} of the same type.");
                }
                else
                {
                    Debug.Log($"[BubbleCollisionChecker] {gameObject.name} overlapped with {otherBubble.name}, but the types are different.");
                }
            }
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
