using UnityEngine;
using UnityEngine.UI;

public class ShowUIAtStart : MonoBehaviour
{
    // Reference to the UI element you want to display
    public GameObject uiElement;

    // Duration the UI element will be visible
    public float displayDuration = 3f;

    // Duration of the fade-out effect
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (uiElement != null)
        {
            canvasGroup = uiElement.GetComponent<CanvasGroup>();

            // If CanvasGroup is not already attached, add it dynamically
            if (canvasGroup == null)
            {
                canvasGroup = uiElement.AddComponent<CanvasGroup>();
            }

            // Ensure the UI element is visible and reset the alpha to 1
            canvasGroup.alpha = 1f;
            uiElement.SetActive(true);

            // Start the coroutine to fade out the UI element after the display duration
            StartCoroutine(FadeOutAfterTime(displayDuration));
        }
    }

    private System.Collections.IEnumerator FadeOutAfterTime(float time)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(time);

        // Fade out over the specified fadeDuration
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // Make sure the alpha is fully 0 after the fade
        canvasGroup.alpha = 0f;

        // Optionally, disable the UI element once the fade-out is complete
        uiElement.SetActive(false);
    }
}
