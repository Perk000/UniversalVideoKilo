using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VideoStreamManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public Button playPauseButton;
    public Button stopButton;
    public Slider timelineSlider;
    public TextMeshProUGUI timeText;

    [Header("Source Inputs")]
    public TMP_InputField youtubeUrlInput;
    public Button loadYoutubeButton;
    public Button cameraRollButton;
    public Button databaseButton;

    [Header("Recording")]
    public Button recordButton;
    // Add NSR components here when available

    private string currentSource = "";
    private bool isPrepared = false;
    private bool isDraggingSlider = false;

    void Start()
    {
        // Setup video player events
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        // Setup UI events
        if (loadYoutubeButton != null)
            loadYoutubeButton.onClick.AddListener(LoadYouTubeVideo);

        if (cameraRollButton != null)
            cameraRollButton.onClick.AddListener(OpenCameraRoll);

        if (databaseButton != null)
            databaseButton.onClick.AddListener(LoadFromDatabase);

        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(PlayPauseVideo);

        if (stopButton != null)
            stopButton.onClick.AddListener(StopVideo);

        if (timelineSlider != null)
            timelineSlider.onValueChanged.AddListener(SetTimeline);

        if (recordButton != null)
            recordButton.onClick.AddListener(StartRecording);

        // Load saved source if any
        currentSource = PlayerPrefs.GetString("videoSource", "");
        if (!string.IsNullOrEmpty(currentSource))
        {
            LoadVideo(currentSource);
        }
    }

    void Update()
    {
        // Update timeline slider
        if (videoPlayer != null && timelineSlider != null && videoPlayer.isPrepared)
        {
            if (!isDraggingSlider)
            {
                timelineSlider.value = (float)(videoPlayer.time / videoPlayer.length);
            }

            if (timeText != null)
            {
                timeText.text = $"{videoPlayer.time:F1}s / {videoPlayer.length:F1}s";
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == timelineSlider.gameObject)
        {
            isDraggingSlider = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDraggingSlider)
        {
            isDraggingSlider = false;
        }
    }

    public void LoadYouTubeVideo()
    {
        if (youtubeUrlInput != null && !string.IsNullOrEmpty(youtubeUrlInput.text))
        {
            string url = youtubeUrlInput.text;
            // For YouTube, we need to extract the video ID and use a streaming service
            // This is a placeholder - in real implementation, use YouTube API or streaming service
            LoadVideo(url);
        }
    }

    public void OpenCameraRoll()
    {
        // Find and use DeviceVideoPickerTest
        DeviceVideoPickerTest picker = FindObjectOfType<DeviceVideoPickerTest>();
        if (picker != null)
        {
            picker.OpenVideoPicker();
        }
        else
        {
            Debug.LogWarning("DeviceVideoPickerTest not found in scene");
        }
    }

    public void LoadFromDatabase()
    {
        // Placeholder for web API integration
        // In full implementation, query database and get video URL
        Debug.Log("Loading from Database - implement web API call");
        string databaseUrl = "https://api.example.com/video/123"; // Placeholder
        LoadVideo(databaseUrl);
    }

    public void LoadVideo(string source)
    {
        currentSource = source;
        PlayerPrefs.SetString("videoSource", source);

        if (videoPlayer != null)
        {
            videoPlayer.url = source;
            videoPlayer.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        isPrepared = true;
        Debug.Log("Video prepared successfully");
        // Auto-play after preparation for efficiency
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished playing");
        // Handle video completion
    }

    public void PlayPauseVideo()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
            else if (isPrepared)
            {
                videoPlayer.Play();
            }
        }
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
    }

    public void SetTimeline(float value)
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    public void StartRecording()
    {
        // Find and use VideoRecorder
        VideoRecorder recorder = FindObjectOfType<VideoRecorder>();
        if (recorder != null)
        {
            if (recorder.IsRecording())
            {
                recorder.StopRecording();
            }
            else
            {
                recorder.StartRecording();
            }
        }
        else
        {
            Debug.LogWarning("VideoRecorder not found in scene");
        }
    }

    // Public methods for external access
    public bool IsVideoPlaying()
    {
        return videoPlayer != null && videoPlayer.isPlaying;
    }

    public double GetCurrentTime()
    {
        return videoPlayer != null ? videoPlayer.time : 0;
    }

    public double GetVideoLength()
    {
        return videoPlayer != null ? videoPlayer.length : 0;
    }
}