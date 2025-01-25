using System.Collections;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    private Vector3 originalPosition; // Original position for floating animation
    private Vector3 offset; // Offset for dragging
    private Camera mainCamera; // Reference to the main Unity camera
    private bool isDragging; // Flag to check if the bubble is being dragged
    private Coroutine floatingCoroutine; // Reference to the floating coroutine
    private Coroutine returnCoroutine; // Reference to the return-to-position coroutine

    private void Awake()
    {
        // Automatically find the camera tagged as "MainCamera"
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No camera tagged as 'MainCamera' found. Please ensure your main camera is properly tagged.");
        }
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        transform.position = new Vector3(position.x, position.y, position.z); // Ensure Z is set explicitly
    }

    public void StartFloating(float speed)
    {
        // Stop any existing floating coroutine before starting a new one
        if (floatingCoroutine != null)
        {
            StopCoroutine(floatingCoroutine);
        }

        floatingCoroutine = StartCoroutine(FloatAnimation(speed));
    }

    private IEnumerator FloatAnimation(float speed)
    {
        while (true)
        {
            float floatOffset = Mathf.Sin(Time.time * speed) * 0.5f; // Adjust 0.5f for height
            // Apply floating on Y-axis only, lock X and Z
            transform.position = new Vector3(originalPosition.x, originalPosition.y + floatOffset, originalPosition.z);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not set. Unable to process mouse click.");
            return;
        }

        // Stop floating animation while being dragged
        if (floatingCoroutine != null)
        {
            StopCoroutine(floatingCoroutine);
            floatingCoroutine = null;
            Debug.Log($"Stopped floating for: {gameObject.name}");
        }

        // If a return coroutine is running, stop it
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        // Log when the thought bubble is clicked
        Debug.Log($"Thought bubble clicked: {gameObject.name}");

        // Start dragging
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPos;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging && mainCamera != null)
        {
            // Log during dragging
            Debug.Log($"Dragging thought bubble: {gameObject.name}");

            // Follow the mouse position while locking Z-axis
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            transform.position = new Vector3(mouseWorldPos.x + offset.x, mouseWorldPos.y + offset.y, originalPosition.z); // Lock Z-axis
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            // Log when dragging stops
            Debug.Log($"Stopped dragging thought bubble: {gameObject.name}");

            // Smoothly return to the original position after dropping
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
            }

            returnCoroutine = StartCoroutine(ReturnToOriginalPosition());
        }

        isDragging = false;
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration to return to the original position
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            // Interpolate between the current position and the original position
            transform.position = Vector3.Lerp(startPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly the original position
        transform.position = originalPosition;

        // Restart the floating animation
        StartFloating(1.5f); // Adjust speed as needed
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not set. Unable to calculate mouse world position.");
            return Vector3.zero;
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z - originalPosition.z); // Use the Z-position of the original position
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}
