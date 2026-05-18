using UnityEngine;
using TMPro;

public class EndLevelRank : MonoBehaviour
{
    public float aSec = 70;
    public float bSec = 80;
    public float cSec = 90;
    public float dSec = 100;

    public TextMeshProUGUI _text;

    private float elapsedTimeFromTimer = 0f;

    public Timer timerScript;

    // Update is called once per frame
    void Update()
    {
        elapsedTimeFromTimer = timerScript.elapsedTime;
        // Determine Rank
        if (elapsedTimeFromTimer >= dSec)
        {
            _text.text = "Rank: D";
        } else if (elapsedTimeFromTimer >= cSec)
        {
            _text.text = "Rank: C";
        } else if (elapsedTimeFromTimer >= bSec)
        {
            _text.text = "Rank: B";
        } else if (elapsedTimeFromTimer >= aSec)
        {
            _text.text = "Rank: A";
        } else
        {
            _text.text = "Rank: S";
        }
    }
}
