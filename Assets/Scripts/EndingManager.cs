using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class EndingManager : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private Transform roundSeal;
    [SerializeField] private Image whiteFadeImage; 
    [SerializeField] private TextMeshProUGUI endingText; 

    [Header("Ceremony Settings")]
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private float maxSpinSpeed = 720f; 
    [SerializeField] private string finalMessage = "THANK YOU FOR PLAYING";

    [Header("Earthquake Settings")]
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private AudioSource endingAudio;
    [SerializeField] private float shakeMagnitude = 0.2f;
    
    private Vector3 originalCameraPos;

    public void StartEndingSequence()
    {
        StartCoroutine(EndingCeremonyRoutine());
    }

    private IEnumerator EndingCeremonyRoutine()
    {
        // Store the original camera position so we can reset it later
        if (mainCameraTransform != null)
        {
            originalCameraPos = mainCameraTransform.localPosition;
        }

        // Play the earthquake sound right as the fade/shake begins
        if (endingAudio != null)
        {
            endingAudio.Play();
        }

        float elapsedTime = 0f;
        if (whiteFadeImage != null)
        {
            Color c = whiteFadeImage.color;
            c.a = 0f;
            whiteFadeImage.color = c;
            whiteFadeImage.gameObject.SetActive(true);
        }

        if (endingText != null)
        {
            endingText.text = finalMessage;
            endingText.color = new Color(endingText.color.r, endingText.color.g, endingText.color.b, 0f); 
            endingText.gameObject.SetActive(true);
        }

        // --- Phase 1: White Fade, Spinning Seal, and Camera Shake ---
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration; 

            // 1. Spin Seal
            if (roundSeal != null)
            {
                float currentSpinSpeed = Mathf.Lerp(0f, maxSpinSpeed, progress);
                roundSeal.Rotate(Vector3.forward * currentSpinSpeed * Time.deltaTime);
            }

            // 2. Fade to White
            if (whiteFadeImage != null)
            {
                Color c = whiteFadeImage.color;
                c.a = progress;
                whiteFadeImage.color = c;
            }

            // 3. Camera Shake (Random offset applied every frame)
            if (mainCameraTransform != null)
            {
                // Generate a random vector inside a sphere and multiply by magnitude
                Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
                // Keep the original Z position so the camera doesn't clip through things awkwardly
                randomOffset.z = 0; 

                mainCameraTransform.localPosition = originalCameraPos + randomOffset;
            }

            yield return null; 
        }

        // Reset camera back to its perfect original spot once the screen is fully white
        if (mainCameraTransform != null)
        {
            mainCameraTransform.localPosition = originalCameraPos;
        }

        yield return new WaitForSeconds(1.5f);

        // --- Phase 2: Fade in Ending Text ---
        float textFadeTime = 2f;
        elapsedTime = 0f;
        
        while (elapsedTime < textFadeTime)
        {
            elapsedTime += Time.deltaTime;
            float textProgress = elapsedTime / textFadeTime;

            if (endingText != null)
            {
                Color textColor = endingText.color;
                textColor.a = textProgress;
                endingText.color = textColor;
            }
            yield return null;
        }

        yield return new WaitForSeconds(5f);
        Debug.Log("Game officially over.");
    }
}
