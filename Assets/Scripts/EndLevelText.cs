using System.Collections; // Required for Coroutines (IEnumerator)
using UnityEngine;

public class EndLevelText : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(WaitAndFadeOut(0.5f, 1.5f)); 
    }

    private IEnumerator WaitAndFadeOut(float delayBeforeFade, float fadeDuration)
    {
        // Wait on screen for a couple of seconds
        yield return new WaitForSeconds(delayBeforeFade);

        // fade the alpha from 1 down to 0
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}
