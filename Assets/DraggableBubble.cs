using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableBubble : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string bubbleType; // Unique identifier for this bubble type

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 initialPosition; // Store the bubble's original position

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Debug: Ensure required components exist
        if (canvas == null)
        {
            Debug.LogError($"[DraggableBubble] No parent Canvas found for {gameObject.name}. Ensure the bubble is inside a Canvas.");
        }
    }

    private void Start()
    {
        // Store the initial position of the bubble
        initialPosition = rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[DraggableBubble] {gameObject.name} clicked.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[DraggableBubble] {gameObject.name} drag started.");

        // Allow the bubble to be dragged smoothly
        canvasGroup.alpha = 0.8f; // Make the bubble slightly transparent during drag
        canvasGroup.blocksRaycasts = false; // Disable raycast blocking during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Drag the bubble based on the mouse position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[DraggableBubble] {gameObject.name} drag ended.");

        // Reset alpha and re-enable raycasting
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Smoothly return the bubble to its initial position
        StartCoroutine(ReturnToInitialPosition());
    }

    private System.Collections.IEnumerator ReturnToInitialPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration to return to the initial position
        Vector3 startPosition = rectTransform.localPosition;

        while (elapsedTime < duration)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, initialPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = initialPosition;
        Debug.Log($"[DraggableBubble] {gameObject.name} returned to its initial position.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if colliding with another bubble and compare bubbleType
        DraggableBubble otherBubble = other.GetComponent<DraggableBubble>();
        if (otherBubble != null && otherBubble.bubbleType == bubbleType)
        {
            Debug.Log($"[DraggableBubble] Bubble of type {bubbleType} collided with another bubble of the same type: {otherBubble.name}");
        }
    }
}
