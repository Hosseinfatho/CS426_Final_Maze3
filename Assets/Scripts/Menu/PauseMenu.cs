using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void pauseGame()
    {
        if (pauseMenu != null)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                resumeGame();
            }
            else
            {
                pauseGame();
            }
        }
    }
}
