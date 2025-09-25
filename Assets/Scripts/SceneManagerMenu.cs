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

    public void LoadFullScreenVideoPlayer(string source)
    {
        PlayerPrefs.SetString("videoSource", source);
        SceneManager.LoadScene("FullScreenVideoPlayer");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void LoadInformationPage()
    {
        SceneManager.LoadScene("InformationPage");
    }

    public void LoadYouTubeGallery()
    {
        SceneManager.LoadScene("YouTubeGallery");
    }

    public void LoadUserVideosGallery()
    {
        SceneManager.LoadScene("UserVideosGallery");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}