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

        // Clear previous choices
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // Create buttons for each choice
        foreach (DialogueChoice choice in choices)
        {
            GameObject choiceButton = Instantiate(choicePrefab, choiceContainer);
            TextMeshProUGUI choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

            if (choiceText == null)
            {
                Debug.LogError("[DialogueManager] ChoicePrefab is missing a TextMeshProUGUI component!");
                return;
            }

            choiceText.text = choice.text;

            // Assign behavior to the button
            Button button = choiceButton.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("[DialogueManager] ChoicePrefab is missing a Button component!");
                return;
            }

            button.onClick.AddListener(() =>
            {
                choice.onSelect?.Invoke(); // Trigger the UnityEvent
                ClearChoices();

                if (choice.isCorrect)
                {
                    // Continue current dialogue
                    Debug.Log("[DialogueManager] Correct choice selected, continuing dialogue.");
                    DisplayNextDialogueLine();
                }
                else if (choice.linkedDialogue != null)
                {
                    // Switch to the linked dialogue
                    Debug.Log($"[DialogueManager] Wrong choice, switching to dialogue: {choice.linkedDialogue.name}");
                    StartDialogue(choice.linkedDialogue);
                }
                else
                {
                    Debug.LogWarning("[DialogueManager] No linked dialogue for this choice.");
                }
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
