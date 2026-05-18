using UnityEngine;

public class EndLevelText : MonoBehaviour
{
    public void Show()
    {
        GetComponent<CanvasGroup>().alpha = 1f; // Make visible
    }

    public void Hide()
    {
        GetComponent<CanvasGroup>().alpha = 0f; // Make invisible
    }
}
