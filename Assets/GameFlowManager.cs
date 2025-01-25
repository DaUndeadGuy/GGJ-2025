using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    [Header("Key Press and Objects")]
    public RectTransform canvasRectTransform; // Canvas RectTransform
    public KeyCode toggleKey = KeyCode.Space; // Key to toggle thought bubbles

    [Header("Object-Bubble Mapping")]
    public List<Transform> targetObjects; // List of target objects
    public List<GameObject> specificPrefabs; // Corresponding prefabs for the target objects

    private Dictionary<Transform, GameObject> objectPrefabMapping = new Dictionary<Transform, GameObject>(); // Mapping of objects to prefabs
    private Dictionary<Transform, GameObject> activeThoughtBubbles = new Dictionary<Transform, GameObject>(); // Tracking active bubbles

    private void Start()
    {
        // Populate the object-prefab dictionary
        if (targetObjects.Count != specificPrefabs.Count)
        {
            Debug.LogError("Target Objects and Specific Prefabs must have the same number of elements.");
            return;
        }

        for (int i = 0; i < targetObjects.Count; i++)
        {
            if (targetObjects[i] != null && specificPrefabs[i] != null)
            {
                objectPrefabMapping[targetObjects[i]] = specificPrefabs[i];
            }
        }
    }

    private void Update()
    {
        // Check for key press to toggle thought bubbles
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleThoughtBubbles();
        }
    }

    private void ToggleThoughtBubbles()
    {
        foreach (var kvp in objectPrefabMapping)
        {
            Transform target = kvp.Key;
            GameObject prefab = kvp.Value;

            if (activeThoughtBubbles.ContainsKey(target))
            {
                // If bubble exists, remove it
                Destroy(activeThoughtBubbles[target]);
                activeThoughtBubbles.Remove(target);
            }
            else
            {
                // If bubble doesn't exist, spawn it
                SpawnThoughtBubble(target, prefab);
            }
        }

        // Update player movement
        UpdatePlayerMovement();
    }

    private void SpawnThoughtBubble(Transform worldTransform, GameObject prefab)
    {
        // Instantiate the specific prefab as a child of the canvas
        GameObject bubble = Instantiate(prefab, canvasRectTransform);

        // Convert the world position to screen space and assign the bubble's position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldTransform.position);
        bubble.GetComponent<RectTransform>().position = screenPosition;

        // Track the bubble in the dictionary
        activeThoughtBubbles[worldTransform] = bubble;
    }

    private void UpdatePlayerMovement()
    {
        // Disable movement if there are active bubbles, enable it otherwise
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.CanMove = activeThoughtBubbles.Count == 0;
        }
    }
}
