using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Transform FocusTarget; // The object to focus on during the cutscene

    [Tooltip("Optional action to execute after the cutscene ends")]
    public UnityEngine.Events.UnityEvent OnCutsceneEnd;

    public void TriggerCutscene()
    {
        // Ensure the CutsceneManager is available
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.FocusOnObject(FocusTarget, OnCutsceneEnd);
        }
        else
        {
            Debug.LogWarning("CutsceneManager is not available in the scene.");
        }
    }
}