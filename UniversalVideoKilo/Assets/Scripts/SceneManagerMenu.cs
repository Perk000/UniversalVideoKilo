using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerMenu : MonoBehaviour
{
    public void LoadVideoSelection()
    {
        SceneManager.LoadScene("VideoSelection");
    }

    public void LoadArVideoPlayer(string source)
    {
        PlayerPrefs.SetString("videoSource", source);
        SceneManager.LoadScene("ArVideoPlayer");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}