using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using SilverTau.NSR.Recorders.Video;

public class VideoPlayerUI : MonoBehaviour, IPointerDownHandler
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
    private DelayPopupController delayPopup;
    private NSRVideoRecorderController nsrRecorder;

    public CanvasGroup uiCanvasGroup;
    public UnityEngine.UI.GraphicRaycaster uiGraphicRaycaster;

    private const float longPressTime = 0.5f; // 0.5 second for long press
    private bool isUpdatingSlider = false;
    private float clickStartTime = -1f;

    public float fadeDuration = 0.5f;
    public float visibleDuration = 3f;
    private float hideTimer = 0f;
    private bool isUIVisible = true;

    void Start()
    {
        videoManager = FindObjectOfType<ArVideoPlayerManager>();
        Debug.Log("VideoPlayerUI: videoManager found: " + (videoManager != null));
        delayPopup = transform.Find("DelayPopup")?.GetComponent<DelayPopupController>();
        nsrRecorder = FindObjectOfType<NSRVideoRecorderController>();
        playPauseButton = playPauseButtonGO?.GetComponent<Button>();
        Debug.Log("playPauseButton: " + (playPauseButton != null) + ", GO: " + (playPauseButtonGO != null));
        recordButton = recordButtonGO?.GetComponent<Button>();
        resetButton = resetButtonGO?.GetComponent<Button>();
        opacitySlider = opacitySliderGO?.GetComponent<Slider>();
        timelineSlider = timelineSliderGO?.GetComponent<Slider>();
        timeClockText = timeClockTextGO?.GetComponent<TextMeshProUGUI>();
        videoNameText = videoNameTextGO?.GetComponent<TextMeshProUGUI>();

        if (opacitySlider) opacitySlider.value = 1; // Initialize to full opacity

        // Set up EventTrigger for playPauseButton long press
        if (playPauseButtonGO)
        {
            EventTrigger trigger = playPauseButtonGO.GetComponent<EventTrigger>();
            if (trigger == null) trigger = playPauseButtonGO.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            // PointerDown
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            trigger.triggers.Add(entryDown);
        }

        if (playPauseButton) playPauseButton.onClick.AddListener(() => { HandlePlayPauseClick(); });
        if (recordButton) recordButton.onClick.AddListener(() => { ToggleRecording(); ShowUI(); });
        if (resetButton) resetButton.onClick.AddListener(() => { Debug.Log("Reset button clicked"); videoManager?.ResetVideo(); ShowUI(); });
        if (opacitySlider) opacitySlider.onValueChanged.AddListener((value) => { Debug.Log("Opacity changed: " + value); videoManager?.SetOpacity(value); ShowUI(); });
        if (timelineSlider) timelineSlider.onValueChanged.AddListener((value) => { if (!isUpdatingSlider) { Debug.Log("Timeline changed: " + value); videoManager?.SetTimeline(value); ShowUI(); } });
        UpdateUI();
        ShowUI(); // Start with UI visible and timer
    }

    void Update()
    {
        // Find videoManager if not found
        if (videoManager == null)
        {
            videoManager = FindObjectOfType<ArVideoPlayerManager>();
        }

        // UI fade timer
        if (isUIVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideUI();
            }
        }

        // Detect mouse click to show UI (for simulator)
        if (Input.GetMouseButtonDown(0))
        {
            ShowUI();
        }

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
                isUpdatingSlider = true;
                timelineSlider.value = vp.length > 0 ? (float)(vp.time / vp.length) : 0;
                isUpdatingSlider = false;
            }
        }
    }

    private void ShowUI()
    {
        Debug.Log("ShowUI called");
        if (!isUIVisible)
        {
            isUIVisible = true;
            if (uiCanvasGroup) uiCanvasGroup.interactable = true;
            if (uiGraphicRaycaster) uiGraphicRaycaster.enabled = true;
            StartCoroutine(FadeUI(1f));
        }
        hideTimer = visibleDuration;
    }

    private void HideUI()
    {
        Debug.Log("HideUI called");
        if (isUIVisible)
        {
            isUIVisible = false;
            StartCoroutine(FadeUI(0f, () => {
                if (uiCanvasGroup) uiCanvasGroup.interactable = false;
                if (uiGraphicRaycaster) uiGraphicRaycaster.enabled = false;
            }));
        }
    }

    private IEnumerator FadeUI(float targetAlpha, System.Action onComplete = null)
    {
        if (!uiCanvasGroup) yield break;
        float startAlpha = uiCanvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        uiCanvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    private void ToggleRecording()
    {
        Debug.Log("ToggleRecording called");
        if (nsrRecorder)
        {
            // Check if NSR is currently recording
            if (UniversalVideoRecorder.Instance != null && UniversalVideoRecorder.Instance.IsRecording)
            {
                nsrRecorder.StopRecording();
                Debug.Log("Stopped NSR recording");
            }
            else
            {
                nsrRecorder.StartRecording();
                Debug.Log("Started NSR recording");
            }
        }
        else
        {
            Debug.LogWarning("NSRVideoRecorderController not found!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickStartTime = Time.time;
        ShowUI();
    }

    private void HandlePlayPauseClick()
    {
        if (Time.time - clickStartTime > longPressTime)
        {
            delayPopup?.ShowPopup();
        }
        else
        {
            videoManager?.PlayPause();
        }
    }
}