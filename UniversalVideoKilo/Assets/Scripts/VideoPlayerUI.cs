using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoPlayerUI : MonoBehaviour
{
    public GameObject playPauseButtonGO;
    public GameObject recordButtonGO; // Placeholder for recording functionality
    public GameObject resetButtonGO;
    public GameObject opacitySliderGO;
    public GameObject timelineSliderGO;
    public GameObject timeClockTextGO;
    public GameObject videoNameTextGO;

    private Button playPauseButton;
    private Button recordButton;
    private Button resetButton;
    private Slider opacitySlider;
    private Slider timelineSlider;
    private TextMeshProUGUI timeClockText;
    private TextMeshProUGUI videoNameText;

    private ArVideoPlayerManager videoManager;

    void Start()
    {
        videoManager = FindObjectOfType<ArVideoPlayerManager>();
        playPauseButton = playPauseButtonGO?.GetComponent<Button>();
        recordButton = recordButtonGO?.GetComponent<Button>();
        resetButton = resetButtonGO?.GetComponent<Button>();
        opacitySlider = opacitySliderGO?.GetComponent<Slider>();
        timelineSlider = timelineSliderGO?.GetComponent<Slider>();
        timeClockText = timeClockTextGO?.GetComponent<TextMeshProUGUI>();
        videoNameText = videoNameTextGO?.GetComponent<TextMeshProUGUI>();

        if (playPauseButton) playPauseButton.onClick.AddListener(() => videoManager?.PlayPause());
        if (resetButton) resetButton.onClick.AddListener(() => videoManager?.ResetVideo());
        if (opacitySlider) opacitySlider.onValueChanged.AddListener((value) => videoManager?.SetOpacity(value));
        if (timelineSlider) timelineSlider.onValueChanged.AddListener((value) => videoManager?.SetTimeline(value));
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (videoManager)
        {
            timeClockText.text = videoManager.GetTimeClock();
            videoNameText.text = videoManager.GetVideoName();
            VideoPlayer vp = videoManager.GetComponent<VideoPlayer>();
            if (vp && timelineSlider)
            {
                timelineSlider.value = vp.length > 0 ? (float)(vp.time / vp.length) : 0;
            }
        }
    }
}