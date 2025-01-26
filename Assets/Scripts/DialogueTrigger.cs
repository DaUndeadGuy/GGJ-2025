using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueChoice
{
    public string text; // Choice text
    public UnityEvent onSelect; // Event triggered when this choice is selected
    public bool isCorrect; // Whether this choice is correct
    public Dialogue linkedDialogue; // Optional linked dialogue for wrong answers
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;

    public List<DialogueChoice> choices = new List<DialogueChoice>(); // Choices for this dialogue line
}

[System.Serializable]
public class Dialogue
{
    public string name;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // Dialogue to trigger
    public UnityEvent onDialogueEnd; // Custom UnityEvent after dialogue ends

    [Header("Tag-Based Control")]
    public string tagToEnable; // Tag for objects to enable
    public string tagToDisable; // Tag for objects to disable

    [Header("Stage Control")]
    public int nextStageIndex = -1; // Next stage index to set in the GameFlowManager (-1 means no stage change)

    public void TriggerDialogue()
    {
        // Start the dialogue in the DialogueManager
        DialogueManager.Instance.StartDialogue(dialogue);

        // Subscribe to the onDialogueEnd event
        DialogueManager.Instance.onDialogueEnd.RemoveListener(OnDialogueEnd); // Prevent duplicates
        DialogueManager.Instance.onDialogueEnd.AddListener(OnDialogueEnd);
    }

    private void OnDialogueEnd()
    {
        // Enable objects with the specified tag
        if (!string.IsNullOrEmpty(tagToEnable))
        {
            List<GameObject> objectsToEnable = FindAllObjectsWithTag(tagToEnable);
            foreach (var obj in objectsToEnable)
            {
                obj.SetActive(true);
                Debug.Log($"[DialogueTrigger] Enabled {obj.name} with tag {tagToEnable}.");
            }
        }

        // Disable objects with the specified tag
        if (!string.IsNullOrEmpty(tagToDisable))
        {
            List<GameObject> objectsToDisable = FindAllObjectsWithTag(tagToDisable);
            foreach (var obj in objectsToDisable)
            {
                obj.SetActive(false);
                Debug.Log($"[DialogueTrigger] Disabled {obj.name} with tag {tagToDisable}.");
            }
        }

        // Change stage if applicable
        if (nextStageIndex >= 0)
        {
            Debug.Log($"[DialogueTrigger] Changing stage to {nextStageIndex}.");
            GameFlowManager.Instance.SetStage(nextStageIndex);
        }

        // Invoke additional custom UnityEvents
        onDialogueEnd?.Invoke();

        Debug.Log("[DialogueTrigger] Dialogue has ended and post-dialogue actions executed.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }

    /// <summary>
    /// Finds all objects with a specific tag, including inactive objects.
    /// </summary>
    private List<GameObject> FindAllObjectsWithTag(string tag)
    {
        List<GameObject> results = new List<GameObject>();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) && obj.hideFlags == HideFlags.None)
            {
                results.Add(obj);
            }
        }

        return results;
    }

    public void ChangeStage(int stage)
    {
        GameFlowManager.Instance.SetStage(stage);
    }
}

