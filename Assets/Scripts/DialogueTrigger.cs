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
    public string text; // The text displayed for the choice
    public UnityEvent onSelect; // The action/event to invoke when the choice is selected
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
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // Dialogue data
    public UnityEvent onDialogueEnd; // Event to trigger after dialogue ends

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);

        // Subscribe the post-dialogue event
        DialogueManager.Instance.onDialogueEnd.RemoveListener(OnDialogueEnd); // Prevent duplicate listeners
        DialogueManager.Instance.onDialogueEnd.AddListener(OnDialogueEnd);
    }

    private void OnDialogueEnd()
    {
        // Trigger the UnityEvent when the dialogue ends
        onDialogueEnd?.Invoke();
        Debug.Log("[DialogueTrigger] Post-dialogue event triggered.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }

    public void ChangeStage(int stage)
    {
        GameFlowManager.Instance.SetStage(stage);
    }
}
