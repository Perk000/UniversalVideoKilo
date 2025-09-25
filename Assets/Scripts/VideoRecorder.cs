using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;

public class VideoRecorder : MonoBehaviour
{
    [Header("Recording UI")]
    public Button recordButton;
    public Button stopRecordButton;
    public Text statusText;
    public Image recordIndicator;

    private bool isRecording = false;
    private string outputPath = "";

    // Import native recording functions (NSR components)
    [DllImport("__Internal")]
    private static extern void _StartScreenRecording(string filePath);

    [DllImport("__Internal")]
    private static extern void _StopScreenRecording();

    [DllImport("__Internal")]
    private static extern bool _IsRecording();

    [DllImport("__Internal")]
    private static extern void _SaveToCameraRoll(string filePath);

    void Start()
    {
        if (recordButton != null)
            recordButton.onClick.AddListener(StartRecording);

        if (stopRecordButton != null)
            stopRecordButton.onClick.AddListener(StopRecording);

        UpdateUI();
    }

    void Update()
    {
        // Update recording indicator
        if (recordIndicator != null)
        {
            recordIndicator.enabled = isRecording;
            if (isRecording)
            {
                // Pulse effect
                float alpha = Mathf.PingPong(Time.time * 2, 1);
                Color color = recordIndicator.color;
                color.a = alpha;
                recordIndicator.color = color;
            }
        }
    }

    public void StartRecording()
    {
        if (isRecording) return;

        // Generate output path
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        outputPath = Path.Combine(Application.persistentDataPath, $"Recording_{timestamp}.mp4");

#if UNITY_IOS && !UNITY_EDITOR
        _StartScreenRecording(outputPath);
        isRecording = true;
#else
        // Simulate recording in editor
        isRecording = true;
        Debug.Log($"Recording started (simulated). Output: {outputPath}");
#endif

        UpdateUI();
    }

    public void StopRecording()
    {
        if (!isRecording) return;

#if UNITY_IOS && !UNITY_EDITOR
        _StopScreenRecording();
        isRecording = false;
#else
        // Simulate stopping recording in editor
        isRecording = false;
        Debug.Log("Recording stopped (simulated)");
#endif

        UpdateUI();

        // Handle recorded video
        if (File.Exists(outputPath))
        {
            Debug.Log($"Recording saved: {outputPath}");

#if UNITY_IOS && !UNITY_EDITOR
            _SaveToCameraRoll(outputPath);
#else
            Debug.Log("Simulated saving to camera roll");
#endif

            // Optionally load the recorded video
            VideoStreamManager streamManager = FindObjectOfType<VideoStreamManager>();
            if (streamManager != null)
            {
                streamManager.LoadVideo("file://" + outputPath);
            }
        }
    }

    private void UpdateUI()
    {
        if (statusText != null)
        {
            statusText.text = isRecording ? "Recording..." : "Ready to record";
        }

        if (recordButton != null)
            recordButton.interactable = !isRecording;

        if (stopRecordButton != null)
            stopRecordButton.interactable = isRecording;
    }

    // Called from native code when recording actually starts/stops
    public void OnRecordingStarted()
    {
        isRecording = true;
        UpdateUI();
        Debug.Log("Recording started");
    }

    public void OnRecordingStopped()
    {
        isRecording = false;
        UpdateUI();
        Debug.Log("Recording stopped");
    }

    public void OnRecordingError(string error)
    {
        isRecording = false;
        UpdateUI();
        Debug.LogError($"Recording error: {error}");

        if (statusText != null)
        {
            statusText.text = $"Recording error: {error}";
        }
    }

    public void OnSavedToCameraRoll()
    {
        Debug.Log("Recording saved to camera roll");
        if (statusText != null)
        {
            statusText.text = "Saved to camera roll";
        }
    }

    public bool IsRecording()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _IsRecording();
#else
        return isRecording;
#endif
    }

    public string GetOutputPath()
    {
        return outputPath;
    }
}