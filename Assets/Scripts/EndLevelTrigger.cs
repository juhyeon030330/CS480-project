using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    public EndLevelText endScript;
    public Timer timerScript;

    // Make timer appear only when ending the level
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timerScript.paused = true;
            endScript.Show();
        }
    }
}
