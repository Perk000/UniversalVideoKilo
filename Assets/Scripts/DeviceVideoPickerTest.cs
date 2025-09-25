using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class DeviceVideoPickerTest : MonoBehaviour
{
    [Header("UI Elements")]
    public Button pickVideoButton;
    public Text statusText;

    // Ensure this GameObject is named "VideoPickerManager" for native callbacks
    void Awake()
    {
        if (gameObject.name != "VideoPickerManager")
        {
            gameObject.name = "VideoPickerManager";
        }
    }

    // Import native iOS functions
    [DllImport("__Internal")]
    private static extern void _OpenVideoPicker();

    [DllImport("__Internal")]
    private static extern string _GetSelectedVideoPath();

    void Start()
    {
        if (pickVideoButton != null)
        {
            pickVideoButton.onClick.AddListener(OpenVideoPicker);
        }

        if (statusText != null)
        {
            statusText.text = "Ready to pick video";
        }
    }

    public void OpenVideoPicker()
    {
        if (statusText != null)
        {
            statusText.text = "Opening video picker...";
        }

#if UNITY_IOS && !UNITY_EDITOR
        _OpenVideoPicker();
#else
        // For testing in editor, simulate selection
        Debug.Log("Video picker opened (simulated in editor)");
        OnVideoSelected("file://simulated/path/video.mp4");
#endif
    }

    // Called from native iOS code when video is selected
    public void OnVideoSelected(string message)
    {
        // Parse message: "path|orientation"
        string[] parts = message.Split('|');
        string videoPath = parts[0];
        int orientation = parts.Length > 1 ? int.Parse(parts[1]) : 0;

        if (statusText != null)
        {
            statusText.text = "Video selected: " + videoPath + " (rotation: " + orientation + "°)";
        }

        Debug.Log("Video selected: " + videoPath + " with orientation: " + orientation);

        // Store orientation for later use
        PlayerPrefs.SetInt("videoOrientation", orientation);

        // Pass the video path to VideoStreamManager
        VideoStreamManager streamManager = FindObjectOfType<VideoStreamManager>();
        if (streamManager != null)
        {
            streamManager.LoadVideo(videoPath);
        }

        // Load the AR video player scene with the selected video
        SceneManagerMenu sceneManager = FindObjectOfType<SceneManagerMenu>();
        if (sceneManager != null)
        {
            sceneManager.LoadArVideoPlayer(videoPath);
        }
    }

    // Called from native iOS code if picker is cancelled
    public void OnVideoPickerCancelled()
    {
        if (statusText != null)
        {
            statusText.text = "Video selection cancelled";
        }

        Debug.Log("Video picker cancelled");
    }

    // Method to get selected video path (for polling if needed)
    public string GetSelectedVideoPath()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _GetSelectedVideoPath();
#else
        return "";
#endif
    }
}