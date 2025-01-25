using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera; // Reference to the existing Cinemachine camera

    private Transform defaultFollow; // Default Follow target
    private Transform defaultLookAt; // Default LookAt target
    private bool isCutsceneActive = false; // Prevent overlapping cutscenes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Save the default Follow and LookAt targets
        defaultFollow = cinemachineCamera.Follow;
        defaultLookAt = cinemachineCamera.LookAt;
    }

    public void FocusOnObject(Transform target, UnityEvent onCutsceneEnd = null, bool resetAfterCutscene = true)
    {
        if (isCutsceneActive) return; // Prevent multiple cutscenes at once

        StartCoroutine(FocusCoroutine(target, onCutsceneEnd, resetAfterCutscene));
    }

    private IEnumerator FocusCoroutine(Transform target, UnityEvent onCutsceneEnd, bool resetAfterCutscene)
    {
        isCutsceneActive = true;

        // Change the camera's Follow and LookAt targets to the target object
        cinemachineCamera.Follow = target;
        cinemachineCamera.LookAt = target;

        // Wait until the dialogue ends
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            yield return null;
        }

        // Reset the camera to its default Follow and LookAt targets if allowed
        if (resetAfterCutscene)
        {
            cinemachineCamera.Follow = defaultFollow;
            cinemachineCamera.LookAt = defaultLookAt;
        }

        isCutsceneActive = false;

        // Invoke any additional actions after the cutscene ends
        onCutsceneEnd?.Invoke();
    }



}