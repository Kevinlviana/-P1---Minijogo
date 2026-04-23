using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level01");
    }

    public void RestartGame()
    {   
        SceneManager.LoadScene("Level01");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
