using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;

    [SerializeField] GameObject pausePanelUI;

    public void Pause()
    {
        pausePanelUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pausePanelUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        pausePanelUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }
    public void Quit()
    {
        Application.Quit();
    }
} 
