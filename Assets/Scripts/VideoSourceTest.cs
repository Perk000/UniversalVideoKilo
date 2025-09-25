using UnityEngine;
using UnityEngine.UI;

public class VideoSourceTest : MonoBehaviour
{
    [Header("Test Components")]
    public VideoStreamManager streamManager;
    public DeviceVideoPickerTest videoPicker;
    public VideoRecorder recorder;

    [Header("Test URLs")]
    public string testYouTubeUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
    public string testDatabaseUrl = "https://api.example.com/video/test";

    [Header("UI")]
    public Button testYouTubeButton;
    public Button testCameraRollButton;
    public Button testDatabaseButton;
    public Button testRecordButton;
    public Text statusText;

    void Start()
    {
        if (testYouTubeButton != null)
            testYouTubeButton.onClick.AddListener(TestYouTube);

        if (testCameraRollButton != null)
            testCameraRollButton.onClick.AddListener(TestCameraRoll);

        if (testDatabaseButton != null)
            testDatabaseButton.onClick.AddListener(TestDatabase);

        if (testRecordButton != null)
            testRecordButton.onClick.AddListener(TestRecording);

        UpdateStatus("Test system ready");
    }

    public void TestYouTube()
    {
        if (streamManager != null)
        {
            streamManager.youtubeUrlInput.text = testYouTubeUrl;
            streamManager.LoadYouTubeVideo();
            UpdateStatus("Testing YouTube video loading...");
        }
        else
        {
            UpdateStatus("VideoStreamManager not found!");
        }
    }

    public void TestCameraRoll()
    {
        if (videoPicker != null)
        {
            videoPicker.OpenVideoPicker();
            UpdateStatus("Testing camera roll picker...");
        }
        else
        {
            UpdateStatus("DeviceVideoPickerTest not found!");
        }
    }

    public void TestDatabase()
    {
        if (streamManager != null)
        {
            streamManager.LoadFromDatabase();
            UpdateStatus("Testing database integration...");
        }
        else
        {
            UpdateStatus("VideoStreamManager not found!");
        }
    }

    public void TestRecording()
    {
        if (recorder != null)
        {
            if (recorder.IsRecording())
            {
                recorder.StopRecording();
                UpdateStatus("Stopping recording test...");
            }
            else
            {
                recorder.StartRecording();
                UpdateStatus("Starting recording test...");
            }
        }
        else
        {
            UpdateStatus("VideoRecorder not found!");
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log("VideoSourceTest: " + message);
    }

    // Called by other components to report results
    public void OnTestResult(string result)
    {
        UpdateStatus("Test result: " + result);
    }
}