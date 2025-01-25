using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicDepthOfField : MonoBehaviour
{
    [Header("Depth of Field Settings")]
    [SerializeField] private Volume postProcessingVolume; // Reference to the global post-processing volume

    private DepthOfField depthOfField;

    private void Start()
    {
        // Ensure the volume has Depth of Field and retrieve it
        if (postProcessingVolume.profile.TryGet(out DepthOfField dof))
        {
            depthOfField = dof;
        }
        else
        {
            Debug.LogError("Depth of Field is not set in the Volume Profile.");
        }
    }

    public void AdjustFocus(Transform target, float transitionDuration = 1f)
    {
        if (depthOfField == null) return;
        StopAllCoroutines();
        StartCoroutine(AdjustFocusCoroutine(target, transitionDuration));
    }

    private IEnumerator AdjustFocusCoroutine(Transform target, float duration)
    {
        if (depthOfField == null) yield break;

        float startFocusDistance = depthOfField.focusDistance.value;
        float targetFocusDistance = Vector3.Distance(Camera.main.transform.position, target.position);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            depthOfField.focusDistance.value = Mathf.Lerp(startFocusDistance, targetFocusDistance, elapsedTime / duration);
            yield return null;
        }

        depthOfField.focusDistance.value = targetFocusDistance;
    }

    public void ResetFocus(float defaultDistance = 10f, float transitionDuration = 1f)
    {
        if (depthOfField == null) return;
        StopAllCoroutines();
        StartCoroutine(ResetFocusCoroutine(defaultDistance, transitionDuration));
    }

    private IEnumerator ResetFocusCoroutine(float defaultDistance, float duration)
    {
        if (depthOfField == null) yield break;

        float startFocusDistance = depthOfField.focusDistance.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            depthOfField.focusDistance.value = Mathf.Lerp(startFocusDistance, defaultDistance, elapsedTime / duration);
            yield return null;
        }

        depthOfField.focusDistance.value = defaultDistance;
    }
}