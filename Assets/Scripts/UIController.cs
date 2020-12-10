using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text timeText;
    public float miliseconds, seconds, minutes;

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        UpdateTime();

    }

    private void UpdateTime()
    {
        miliseconds += 0.02f;

        if (miliseconds >= 1)
        {
            seconds++;
            miliseconds = 0;
        }
        if (seconds >= 60)
        {
            minutes++;
            seconds = 0;
        }

        if (minutes > 0)
        {
            if (seconds >= 10)
            {
                timeText.text = "Time: " + minutes.ToString() + ":" + seconds.ToString();
            }
            else
            {
                timeText.text = "Time: " + minutes.ToString() + ":0" + seconds.ToString();
            }
        }
        else
        {

            timeText.text = "Time: " + seconds.ToString();
        }
    }
}
