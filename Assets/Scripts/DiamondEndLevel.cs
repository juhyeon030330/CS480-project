using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiamondEnding : MonoBehaviour
{
    [Header("Player Lock")]
    public GameObject player;
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerLookScript;

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
    private bool endingStarted = false;

    private void Start()
    {
        if (whiteFadeImage != null)
        {
            whiteFadeImage.gameObject.SetActive(false);
        }

        if (endingText != null)
        {
            endingText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (endingStarted)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        endingStarted = true;

        LockPlayer();

        StartCoroutine(EndingCeremonyRoutine());
    }

    private void LockPlayer()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (playerLookScript != null)
        {
            playerLookScript.enabled = false;
        }

        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    private IEnumerator EndingCeremonyRoutine()
    {
        if (mainCameraTransform != null)
        {
            originalCameraPos = mainCameraTransform.localPosition;
        }

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
            Color textColor = endingText.color;
            textColor.a = 0f;
            endingText.color = textColor;
            endingText.gameObject.SetActive(true);
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            if (roundSeal != null)
            {
                float currentSpinSpeed = Mathf.Lerp(0f, maxSpinSpeed, progress);
                roundSeal.Rotate(Vector3.forward * currentSpinSpeed * Time.deltaTime);
            }

            if (whiteFadeImage != null)
            {
                Color c = whiteFadeImage.color;
                c.a = progress;
                whiteFadeImage.color = c;
            }

            if (mainCameraTransform != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
                randomOffset.z = 0f;
                mainCameraTransform.localPosition = originalCameraPos + randomOffset;
            }

            yield return null;
        }

        if (mainCameraTransform != null)
        {
            mainCameraTransform.localPosition = originalCameraPos;
        }

        yield return new WaitForSeconds(1.5f);

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

        Debug.Log("Game officially over.");
    }
}