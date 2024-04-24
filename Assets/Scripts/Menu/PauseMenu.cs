using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    GameObject messageCanvas;

    // pass with timeScale = 1 to not pause the game
    // but still display the menu
    public void pauseGame(float timeScale = 0)
    {
        if (pauseMenu != null)
        {
            Time.timeScale = timeScale;
            pauseMenu.SetActive(true);
        }
    }

    // Color is in 0-1 range float !
    public void showMessage(string message, float r = 1, float g = 1, float b = 1)
    {
        TMP_Text textField = messageCanvas.GetComponentInChildren<TMP_Text>();

        textField.color = new Color(r, g, b);
        textField.text = message;

        messageCanvas.SetActive(true);
        pauseGame();
    }

    public void hideMessage()
    {
        messageCanvas.SetActive(false);
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
        messageCanvas = transform.parent.Find("PauseMenuScreen/MessageCanvas").gameObject;
        messageCanvas.SetActive(false);
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
