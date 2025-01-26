using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; } // Singleton instance

    [Header("Key Press and Objects")]
    public RectTransform canvasRectTransform; // Canvas RectTransform
    public KeyCode toggleKey = KeyCode.Space; // Key to toggle thought bubbles

    [Header("Stage-Based Bubbles")]
    public List<StageConfiguration> stages; // List of stage configurations
    private int currentStage = 0; // Current stage index
    private Dictionary<Transform, GameObject> activeThoughtBubbles = new Dictionary<Transform, GameObject>(); // Tracking active bubbles

    private void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        if (stages.Count == 0)
        {
            Debug.LogError("No stages configured in GameFlowManager.");
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

    public void SetStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stages.Count)
        {
            Debug.LogError($"Invalid stage index: {stageIndex}. Total stages: {stages.Count}");
            return;
        }

        Debug.Log($"Switching to stage {stageIndex}...");
        currentStage = stageIndex;
        UpdateStageBubbles();
    }

    private void UpdateStageBubbles()
    {
        // Clear existing bubbles
        foreach (var bubble in activeThoughtBubbles.Values)
        {
            Destroy(bubble);
        }
        activeThoughtBubbles.Clear();

        // Load new bubbles based on the current stage
        var stageConfig = stages[currentStage];
        for (int i = 0; i < stageConfig.targetObjects.Count; i++)
        {
            Transform target = stageConfig.targetObjects[i];
            GameObject prefab = stageConfig.specificPrefabs[i];

            if (target != null && prefab != null)
            {
                SpawnThoughtBubble(target, prefab);
            }
            else
            {
                Debug.LogError($"Invalid target or prefab at index {i} in stage {currentStage}");
            }
        }

        Debug.Log($"Updated to stage {currentStage}. Loaded {stages[currentStage].targetObjects.Count} bubbles.");
        UpdatePlayerMovement();
    }

    private void ToggleThoughtBubbles()
    {
        if (activeThoughtBubbles.Count > 0)
        {
            // Remove all active bubbles
            foreach (var bubble in activeThoughtBubbles.Values)
            {
                Destroy(bubble);
            }

            activeThoughtBubbles.Clear();
            Debug.Log("All thought bubbles removed.");
        }
        else
        {
            // Reload current stage bubbles
            UpdateStageBubbles();
        }

        UpdatePlayerMovement();
    }

    private void SpawnThoughtBubble(Transform worldTransform, GameObject prefab)
    {
        // Instantiate the specific prefab as a child of the canvas
        GameObject bubble = Instantiate(prefab, canvasRectTransform);

        // Convert the world position to screen space and assign the bubble's position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldTransform.position);

        if (bubble.TryGetComponent<RectTransform>(out RectTransform bubbleRectTransform))
        {
            bubbleRectTransform.position = screenPosition;
        }
        else
        {
            Debug.LogError($"Spawned bubble {bubble.name} is missing a RectTransform component.");
            return;
        }

        // Track the bubble in the dictionary
        activeThoughtBubbles[worldTransform] = bubble;

        Debug.Log($"Spawned bubble {bubble.name} at {screenPosition} for target {worldTransform.name}.");
    }

    private void UpdatePlayerMovement()
    {
        bool hasActiveBubbles = activeThoughtBubbles.Count > 0;

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.CanMove = !hasActiveBubbles;
            Debug.Log($"Player movement set to {(PlayerController.Instance.CanMove ? "enabled" : "disabled")}.");
        }
        else
        {
            Debug.LogError("PlayerController.Instance is null. Ensure the PlayerController script is active and initialized.");
        }
    }
}

[System.Serializable]
public class StageConfiguration
{
    public List<Transform> targetObjects; // List of target objects for this stage
    public List<GameObject> specificPrefabs; // Corresponding prefabs for the target objects
}
