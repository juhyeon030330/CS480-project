using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Setup")]
    [Tooltip("The exact name of the scene you want to load")]
    public string sceneToLoad;

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hello");
        if (other.CompareTag("Player"))
        {
            Debug.Log("hello2");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
