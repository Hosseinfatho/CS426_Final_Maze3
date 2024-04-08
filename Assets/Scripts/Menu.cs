using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void exitGame()
    {
        Application.Quit();
    }

    public void playerLevel1()
    {
        SceneManager.LoadScene("Peter_Assignment6");
    }
}
