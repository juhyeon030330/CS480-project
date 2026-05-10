using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;
    
    [HideInInspector] // We hide this because the Trigger will provide the sentences
    public string[] sentences; 
    
    private int index;
    private bool isTyping = false;
    
    public AudioClip clickSound;
    private AudioSource mySpeaker;

    void Start()
    {
        dialoguePanel.SetActive(false);
        mySpeaker = GetComponent<AudioSource>();
        if (mySpeaker == null) {
            mySpeaker = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        if (dialoguePanel.activeSelf && !isTyping)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (clickSound != null && mySpeaker != null) 
                {
                    mySpeaker.PlayOneShot(clickSound);
                }
                NextSentence();
            }
        }
    }

    // New version: The trigger calls this and passes its own list of sentences
    public void StartDialogue(string[] newSentences)
    {
        // Don't interrupt if someone is already talking
        if (dialoguePanel.activeSelf) return; 

        sentences = newSentences;
        index = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(Type());
    }

    System.Collections.IEnumerator Type()
    {
        isTyping = true;
        textDisplay.text = "";
        
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
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
}