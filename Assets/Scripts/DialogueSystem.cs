using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour 
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;
    
    [Header("Audio Clips")]
    public AudioClip clickSound;     
    public AudioClip typingSound;    

    [Header("Chatter Settings")]
    [Range(0.1f, 0.3f)] public float pitchVariation = 0.15f; 
    public int soundFrequency = 2;
    [Range(0f, 1f)] public float typingVolume = 0.5f;

    private AudioSource uiSource;    // For Enter/Clicks
    private AudioSource speechSource; // For Typing Chatter
    
    private bool isTyping = false;
    private bool isWaitingForNext = false;
    [HideInInspector]
    public string[] sentences;
    private int index;

    void Start() 
    {
        dialoguePanel.SetActive(false);
        
        // Setup two distinct sources
        uiSource = gameObject.AddComponent<AudioSource>();
        speechSource = gameObject.AddComponent<AudioSource>();
        speechSource.volume = typingVolume;

        // Ensure the UI click is always "normal"
        uiSource.pitch = 1.0f;
    }

    void Update() 
    {
        if (dialoguePanel.activeSelf && !isTyping && !isWaitingForNext) 
        {
            if (Input.GetKeyDown(KeyCode.Return)) 
            {
                StartCoroutine(HandleNextSentenceWithDelay());
            }
        }
    }

    private System.Collections.IEnumerator HandleNextSentenceWithDelay()
    {
        isWaitingForNext = true;
        if (clickSound != null)
        {
            uiSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length-0.5f);
        }

        isWaitingForNext = false;
        NextSentence();
    }

    System.Collections.IEnumerator Type() 
    {
        isTyping = true;
        textDisplay.text = "";
        int charCount = 0;

        foreach (char letter in sentences[index].ToCharArray()) 
        {
            textDisplay.text += letter;

            if (letter != ' ' && charCount % soundFrequency == 0)
            {
                if (typingSound != null)
                {
                    // This only affects the speechSource, leaving uiSource alone!
                    speechSource.pitch = Random.Range(1.0f - pitchVariation, 1.0f + pitchVariation);
                    speechSource.PlayOneShot(typingSound);
                }
            }

            charCount++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void NextSentence() 
    {
        if (index < sentences.Length - 1) 
        {
            index++;
            StartCoroutine(Type());
        } 
        else 
        {
            dialoguePanel.SetActive(false);
            textDisplay.text = "";
        }
    }

    public void StartDialogue(string[] newSentences) 
    {
        if (dialoguePanel.activeSelf) return;
        sentences = newSentences;
        index = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(Type());
    }

}
