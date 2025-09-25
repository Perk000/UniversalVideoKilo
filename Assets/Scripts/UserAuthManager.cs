using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class UserAuthManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public TextMeshProUGUI statusText;

    private string apiUrl = "https://your-api-endpoint.com/login"; // Placeholder for web API

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter username and password.";
            return;
        }

        StartCoroutine(LoginRequest(username, password));
    }

    IEnumerator LoginRequest(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse response, e.g., JWT token
                string response = www.downloadHandler.text;
                // Store token, load next scene
                PlayerPrefs.SetString("authToken", response);
                UnityEngine.SceneManagement.SceneManager.LoadScene("VideoSelection");
            }
            else
            {
                statusText.text = "Login failed: " + www.error;
            }
        }
    }
}