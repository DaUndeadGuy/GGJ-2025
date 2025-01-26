using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public Transform choiceContainer; // UI container for choice buttons
    public GameObject choicePrefab; // Prefab for a choice button

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.02f;

    public Animator animator;

    [Header("Events")]
    public UnityEvent onDialogueEnd; // Event to trigger after dialogue ends

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0)
        {
            Debug.LogError("[DialogueManager] Dialogue is empty or not assigned!");
            return;
        }

        isDialogueActive = true;
        PlayerController.Instance.CanMove = false; // Disable player movement
        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            lines.Enqueue(line);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        if (currentLine.character != null)
        {
            characterIcon.sprite = currentLine.character.icon;
            characterName.text = currentLine.character.name;
        }
        else
        {
            Debug.LogWarning("[DialogueManager] DialogueLine has no character assigned.");
            characterIcon.sprite = null;
            characterName.text = "";
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // After typing the line, display choices (if any)
        if (dialogueLine.choices != null && dialogueLine.choices.Count > 0)
        {
            Debug.Log("[DialogueManager] Showing choices...");
            ShowChoices(dialogueLine.choices);
        }
    }

    private void ShowChoices(List<DialogueChoice> choices)
    {
        if (choiceContainer == null)
        {
            Debug.LogError("[DialogueManager] Choice container is not assigned!");
            return;
        }

        if (choicePrefab == null)
        {
            Debug.LogError("[DialogueManager] Choice prefab is not assigned!");
            return;
        }

        // Clear any existing choices
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        if (choices.Count == 0)
        {
            Debug.LogWarning("[DialogueManager] No choices available for this dialogue line.");
            return;
        }

        // Create a button for each choice
        foreach (DialogueChoice choice in choices)
        {
            GameObject choiceButton = Instantiate(choicePrefab, choiceContainer);
            if (choiceButton == null)
            {
                Debug.LogError("[DialogueManager] Failed to instantiate choicePrefab!");
                return;
            }

            Debug.Log($"[DialogueManager] Instantiated choice: {choice.text}");

            TextMeshProUGUI choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (choiceText == null)
            {
                Debug.LogError("[DialogueManager] ChoicePrefab is missing a TextMeshProUGUI component!");
                return;
            }

            choiceText.text = choice.text;

            // Assign the UnityEvent to the button's onClick event
            Button button = choiceButton.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("[DialogueManager] ChoicePrefab is missing a Button component!");
                return;
            }

            button.onClick.AddListener(() =>
            {
                choice.onSelect?.Invoke(); // Execute the assigned action
                ClearChoices();
                DisplayNextDialogueLine();
            });
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        PlayerController.Instance.CanMove = true; // Re-enable player movement
        animator.Play("hide");
        ClearChoices();

        // Trigger post-dialogue events
        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }

        Debug.Log("[DialogueManager] Dialogue ended.");
    }
}
