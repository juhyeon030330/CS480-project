using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI _text;

    public float elapsedTime = 0f;

    public bool paused = false;

    // Update is called once per frame
    void Update()
    {
        if (paused == false)
        {
            elapsedTime += Time.deltaTime;
        }
        _text.text = elapsedTime.ToString("F0") + " seconds";
    }
}
