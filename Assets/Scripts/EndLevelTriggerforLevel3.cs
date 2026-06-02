using UnityEngine;

public class EndLevelTriggerforLevel3 : MonoBehaviour
{
    public EndLevelText endScript;
    public Timer timerScript;

    [Header("Objects to Unlock")]
    public GameObject glassBox;

    [Header("Requirements")]
    public string keyTag = "Key";
    public string enemyTag = "Enemy";

    [Header("Optional UI Message")]
    public GameObject lockedMessage;

    private bool completed = false;

    private void Start()
    {
        if (lockedMessage != null)
        {
            lockedMessage.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (completed)
        {
            return;
        }

        int keysLeft = GameObject.FindGameObjectsWithTag(keyTag).Length;
        int enemiesLeft = CountLivingEnemies();

        if (keysLeft == 0 && enemiesLeft == 0)
        {
            completed = true;

            if (timerScript != null)
            {
                timerScript.paused = true;
            }

            if (endScript != null)
            {
                endScript.Show();
            }

            if (glassBox != null)
            {
                glassBox.SetActive(false);
            }

            Debug.Log("Level requirements complete. Timer/rank shown and diamond unlocked.");
        }
        else
        {
            Debug.Log("Level not complete. Keys left: " + keysLeft + ", enemies left: " + enemiesLeft);

            if (lockedMessage != null)
            {
                lockedMessage.SetActive(true);
            }
        }
    }

    private int CountLivingEnemies()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(enemyTag);
        int count = 0;

        foreach (GameObject enemy in enemyObjects)
        {
            DummyBehavior dummy = enemy.GetComponent<DummyBehavior>();

            if (dummy != null)
            {
                count++;
                Debug.Log("Enemy still counted: " + enemy.name);
            }
        }

        return count;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && lockedMessage != null)
        {
            lockedMessage.SetActive(false);
        }
    }
}