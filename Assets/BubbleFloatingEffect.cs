using UnityEngine;

public class BubbleFloatingEffect : MonoBehaviour
{
    public float floatAmplitude = 10f; // The height of the float movement
    public float floatSpeed = 2f; // The speed of the floating animation
    private RectTransform rectTransform; // Reference to the bubble's RectTransform
    private Vector3 initialPosition; // Initial position of the RectTransform
    private bool isDragging = false; // Flag to check if the bubble is being dragged

    private void Awake()
    {
        // Get the RectTransform of the bubble
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"[BubbleFloatingEffect] No RectTransform found on {gameObject.name}.");
        }
    }

    private void Start()
    {
        // Store the initial local position of the bubble
        if (rectTransform != null)
        {
            initialPosition = rectTransform.localPosition;
        }
    }

    private void Update()
    {
        if (rectTransform == null || isDragging) return;

        // Apply the floating animation only if the bubble is not being dragged
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        rectTransform.localPosition = initialPosition + new Vector3(0, yOffset, 0);
    }

    /// <summary>
    /// Call this method when dragging starts.
    /// </summary>
    public void OnDragStart()
    {
        isDragging = true;
    }

    /// <summary>
    /// Call this method when dragging ends.
    /// </summary>
    public void OnDragEnd()
    {
        isDragging = false;
        // Reset the position to ensure floating resumes correctly
        initialPosition = rectTransform.localPosition;
    }
}
