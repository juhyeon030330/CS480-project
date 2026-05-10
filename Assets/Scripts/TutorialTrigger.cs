using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public DialogueSystem dialogueManager; // Point this to your ONE manager
    [TextArea(3, 10)]
    public string[] myCustomSentences; // Put this trigger's text here!

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dialogueManager.dialoguePanel.activeSelf)
        {
            // Send the specific sentences for THIS trigger to the manager
            dialogueManager.sentences = myCustomSentences;
            dialogueManager.StartDialogue(myCustomSentences); 
            
            // Disable the trigger so it doesn't repeat
            GetComponent<Collider>().enabled = false;
        }
    }
}