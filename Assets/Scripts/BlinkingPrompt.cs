using UnityEngine;
using TMPro;

public class BlinkingPrompt : MonoBehaviour {
    private TextMeshProUGUI text;
    void Start() => text = GetComponent<TextMeshProUGUI>();
    void Update() {
        // Creates a smooth fading effect
        float alpha = (Mathf.Sin(Time.time * 5f) + 1.2f) / 2f;
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}

