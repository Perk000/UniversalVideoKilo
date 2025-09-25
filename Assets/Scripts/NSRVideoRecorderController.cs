using UnityEngine;
using SilverTau.NSR.Recorders.Video;
using SilverTau.NSR.Utilities;
using System.IO;

public class NSRVideoRecorderController : MonoBehaviour
{
    void Start()
    {
        // Set output path for recordings (must be done before recording starts)
        // Note: CustomOutputVideoFilePath is set on the instance, not static
        if (UniversalVideoRecorder.Instance != null)
        {
            // This property is accessed through the instance
            // The path will be used when StartVideoRecorder is called
            Debug.Log("NSR Controller initialized");
        }

        // Subscribe to recording events
        if (UniversalVideoRecorder.Instance != null)
        {
            UniversalVideoRecorder.Instance.onCompleteCapture += OnRecordingComplete;
            UniversalVideoRecorder.Instance.onErrorCapture += OnRecordingError;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (UniversalVideoRecorder.Instance != null)
        {
            UniversalVideoRecorder.Instance.onCompleteCapture -= OnRecordingComplete;
            UniversalVideoRecorder.Instance.onErrorCapture -= OnRecordingError;
        }
    }

    public void StartRecording()
    {
        if (UniversalVideoRecorder.Instance != null)
        {
            // Set custom output path when starting recording
            string customPath = Path.Combine(Application.persistentDataPath, "NSR_Recordings");
            UniversalVideoRecorder.Instance.StartVideoRecorder(customPath);
            Debug.Log("NSR Recording started with custom path: " + customPath);
        }
    }

    public void StopRecording()
    {
        if (UniversalVideoRecorder.Instance != null)
        {
            UniversalVideoRecorder.Instance.StopVideoRecorder();
            Debug.Log("NSR Recording stopped");
        }
    }

    private void OnRecordingComplete()
    {
        Debug.Log("NSR Recording completed successfully");

        // Get the output path
        var path = UniversalVideoRecorder.Instance.VideoOutputPath;
        Debug.Log($"Recording saved to: {path}");

        // Save to Camera Roll on iOS
        if (!string.IsNullOrEmpty(path))
        {
#if UNITY_IOS && !UNITY_EDITOR
            Gallery.SaveVideoToGallery(path);
            Debug.Log("Video saved to iOS Camera Roll");
#endif
        }

        // Optionally load the recorded video into VideoStreamManager
        VideoStreamManager streamManager = FindObjectOfType<VideoStreamManager>();
        if (streamManager != null && !string.IsNullOrEmpty(path))
        {
            streamManager.LoadVideo("file://" + path);
        }
    }

    private void OnRecordingError(string error)
    {
        Debug.LogError($"NSR Recording error: {error}");
    }
}