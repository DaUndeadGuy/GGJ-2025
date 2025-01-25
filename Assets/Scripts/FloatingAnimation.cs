using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    private Vector3 startPosition; // Starting position of the bubble
    public float floatStrength = 0.5f; // Strength of the floating effect
    public float floatSpeed = 1.5f; // Speed of the floating effect

    private RectTransform rectTransform; // Reference to RectTransform (for UI)

    private void Start()
    {
        // Check if the object has a RectTransform (UI element)
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            startPosition = rectTransform.localPosition; // Use RectTransform for UI
        }
        else
        {
            startPosition = transform.localPosition; // Use Transform for non-UI objects
        }
    }

    private void Update()
    {
        if (rectTransform != null)
        {
            // Animate UI RectTransform
            rectTransform.localPosition = new Vector3(
                startPosition.x + Mathf.Sin(Time.time * floatSpeed / 2) * (floatStrength / 2), // Horizontal float
                startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength,          // Vertical float
                startPosition.z
            );
        }
        else
        {
            // Animate non-UI Transform
            transform.localPosition = new Vector3(
                startPosition.x + Mathf.Sin(Time.time * floatSpeed / 2) * (floatStrength / 2), // Horizontal float
                startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength,          // Vertical float
                startPosition.z
            );
        }
    }
}
