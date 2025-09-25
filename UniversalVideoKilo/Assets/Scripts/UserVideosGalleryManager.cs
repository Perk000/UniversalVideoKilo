using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UserVideosGalleryManager : MonoBehaviour
{
    [Header("UI Components")]
    public ScrollRect galleryScrollRect;
    public Transform videoGridContainer;

    [Header("Sorting Options")]
    public Button sortByViewsButton;
    public Button sortByDateButton;
    public Button sortByTitleButton;
    public TextMeshProUGUI sortIndicatorText;

    [Header("Navigation")]
    public Button backButton;
    public Button informationButton;
    public Button youtubeGalleryButton;
    public Button recordNewVideoButton;

    [Header("Statistics")]
    public TextMeshProUGUI totalVideosText;
    public TextMeshProUGUI totalViewsText;
    public TextMeshProUGUI averageRatingText;

    private SceneManagerMenu sceneManager;
    private List<UserVideoData> userVideos = new List<UserVideoData>();
    private SortOption currentSortOption = SortOption.Views;

    public enum SortOption
    {
        Views,
        Date,
        Title
    }

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        // Setup UI events
        if (backButton) backButton.onClick.AddListener(OnBack);
        if (informationButton) informationButton.onClick.AddListener(OnInformation);
        if (youtubeGalleryButton) youtubeGalleryButton.onClick.AddListener(OnYouTubeGallery);
        if (recordNewVideoButton) recordNewVideoButton.onClick.AddListener(OnRecordNewVideo);

        // Setup sorting buttons
        if (sortByViewsButton) sortByViewsButton.onClick.AddListener(() => SortVideos(SortOption.Views));
        if (sortByDateButton) sortByDateButton.onClick.AddListener(() => SortVideos(SortOption.Date));
        if (sortByTitleButton) sortByTitleButton.onClick.AddListener(() => SortVideos(SortOption.Title));

        // Load user videos
        LoadUserVideos();
    }

    void LoadUserVideos()
    {
        // Clear existing videos
        ClearVideoGrid();

        // Load videos from PlayerPrefs or database (mock data for now)
        userVideos = GetMockUserVideos();

        // Sort and display videos
        SortVideos(currentSortOption);
        UpdateStatistics();
    }

    void ClearVideoGrid()
    {
        foreach (Transform child in videoGridContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayVideos(List<UserVideoData> videos)
    {
        foreach (UserVideoData video in videos)
        {
            GameObject videoItem = CreateVideoItem();
            videoItem.transform.SetParent(videoGridContainer, false);
            VideoItemController controller = videoItem.GetComponent<VideoItemController>();

            if (controller)
            {
                controller.SetupUserVideo(video);
            }
        }
    }

    void SortVideos(SortOption sortOption)
    {
        currentSortOption = sortOption;

        switch (sortOption)
        {
            case SortOption.Views:
                userVideos = userVideos.OrderByDescending(v => v.viewCount).ToList();
                if (sortIndicatorText) sortIndicatorText.text = "Sorted by Views";
                break;
            case SortOption.Date:
                // Assuming videos have creation dates, sort by newest first
                userVideos = userVideos.OrderByDescending(v => v.title).ToList(); // Placeholder sort
                if (sortIndicatorText) sortIndicatorText.text = "Sorted by Date";
                break;
            case SortOption.Title:
                userVideos = userVideos.OrderBy(v => v.title).ToList();
                if (sortIndicatorText) sortIndicatorText.text = "Sorted by Title";
                break;
        }

        ClearVideoGrid();
        DisplayVideos(userVideos);

        // Update button states
        UpdateSortButtonStates();
    }

    void UpdateSortButtonStates()
    {
        ColorBlock normalColors = sortByViewsButton.colors;
        normalColors.normalColor = Color.white;

        ColorBlock selectedColors = sortByViewsButton.colors;
        selectedColors.normalColor = new Color(0.2f, 0.6f, 1.0f);

        if (sortByViewsButton) sortByViewsButton.colors = (currentSortOption == SortOption.Views) ? selectedColors : normalColors;
        if (sortByDateButton) sortByDateButton.colors = (currentSortOption == SortOption.Date) ? selectedColors : normalColors;
        if (sortByTitleButton) sortByTitleButton.colors = (currentSortOption == SortOption.Title) ? selectedColors : normalColors;
    }

    void UpdateStatistics()
    {
        int totalVideos = userVideos.Count;
        int totalViews = userVideos.Sum(v => v.viewCount);

        if (totalVideosText) totalVideosText.text = $"Total Videos: {totalVideos}";
        if (totalViewsText) totalViewsText.text = $"Total Views: {totalViews.ToString("N0")}";
        if (averageRatingText) averageRatingText.text = $"Avg Rating: 4.5★"; // Mock rating
    }

    // Navigation methods
    void OnBack()
    {
        if (sceneManager) sceneManager.LoadMainMenu();
    }

    void OnInformation()
    {
        if (sceneManager) sceneManager.LoadInformationPage();
    }

    void OnYouTubeGallery()
    {
        if (sceneManager) sceneManager.LoadYouTubeGallery();
    }

    void OnRecordNewVideo()
    {
        if (sceneManager) sceneManager.LoadVideoSelection();
    }

    // Mock data for demonstration
    List<UserVideoData> GetMockUserVideos()
    {
        return new List<UserVideoData>
        {
            new UserVideoData {
                title = "My First AR Recording",
                description = "Testing the AR video recording feature",
                viewCount = 150,
                duration = "2:30",
                localPath = "file://path/to/video1.mp4"
            },
            new UserVideoData {
                title = "AR Dance Performance",
                description = "Dancing in augmented reality space",
                viewCount = 2500,
                duration = "4:15",
                localPath = "file://path/to/video2.mp4"
            },
            new UserVideoData {
                title = "Product Demo in AR",
                description = "Showcasing products using AR video",
                viewCount = 850,
                duration = "3:45",
                localPath = "file://path/to/video3.mp4"
            },
            new UserVideoData {
                title = "AR Tutorial for Friends",
                description = "Teaching friends how to use AR video",
                viewCount = 75,
                duration = "6:20",
                localPath = "file://path/to/video4.mp4"
            },
            new UserVideoData {
                title = "Virtual Concert Experience",
                description = "Attending a concert through AR video",
                viewCount = 1200,
                duration = "5:10",
                localPath = "file://path/to/video5.mp4"
            }
        };
    }

    // Helper method to create video item at runtime
    GameObject CreateVideoItem()
    {
        GameObject videoItem = new GameObject("VideoItem");
        RectTransform rectTransform = videoItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 200);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        videoItem.AddComponent<CanvasRenderer>();
        Image background = videoItem.AddComponent<Image>();
        background.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Thumbnail
        GameObject thumbnailObj = new GameObject("Thumbnail");
        thumbnailObj.transform.SetParent(videoItem.transform);
        RectTransform thumbRect = thumbnailObj.AddComponent<RectTransform>();
        thumbRect.anchorMin = new Vector2(0, 0.6f);
        thumbRect.anchorMax = new Vector2(1, 1);
        thumbRect.offsetMin = new Vector2(0, 0);
        thumbRect.offsetMax = new Vector2(0, 0);

        RawImage thumbnail = thumbnailObj.AddComponent<RawImage>();
        thumbnail.color = Color.white;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(videoItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.4f);
        titleRect.anchorMax = new Vector2(1, 0.6f);
        titleRect.offsetMin = new Vector2(5, 0);
        titleRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Video Title";
        titleText.fontSize = 14;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.TopLeft;

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(videoItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0.2f);
        descRect.anchorMax = new Vector2(1, 0.4f);
        descRect.offsetMin = new Vector2(5, 0);
        descRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Video description";
        descText.fontSize = 12;
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // View Count
        GameObject viewObj = new GameObject("ViewCount");
        viewObj.transform.SetParent(videoItem.transform);
        RectTransform viewRect = viewObj.AddComponent<RectTransform>();
        viewRect.anchorMin = new Vector2(0, 0.1f);
        viewRect.anchorMax = new Vector2(0.5f, 0.2f);
        viewRect.offsetMin = new Vector2(5, 0);
        viewRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI viewText = viewObj.AddComponent<TextMeshProUGUI>();
        viewText.text = "1.2K views";
        viewText.fontSize = 10;
        viewText.color = new Color(0.4f, 0.4f, 0.4f);
        viewText.alignment = TextAlignmentOptions.TopLeft;

        // Duration
        GameObject durationObj = new GameObject("Duration");
        durationObj.transform.SetParent(videoItem.transform);
        RectTransform durationRect = durationObj.AddComponent<RectTransform>();
        durationRect.anchorMin = new Vector2(0.5f, 0.1f);
        durationRect.anchorMax = new Vector2(1, 0.2f);
        durationRect.offsetMin = new Vector2(0, 0);
        durationRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI durationText = durationObj.AddComponent<TextMeshProUGUI>();
        durationText.text = "5:30";
        durationText.fontSize = 10;
        durationText.color = new Color(0.4f, 0.4f, 0.4f);
        durationText.alignment = TextAlignmentOptions.TopRight;

        // Play Button
        GameObject buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(videoItem.transform);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(1, 0.1f);
        buttonRect.offsetMin = new Vector2(0, 0);
        buttonRect.offsetMax = new Vector2(0, 0);

        Button playButton = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f);

        // Add controller
        VideoItemController controller = videoItem.AddComponent<VideoItemController>();
        controller.thumbnailImage = thumbnail;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.viewCountText = viewText;
        controller.durationText = durationText;
        controller.playButton = playButton;

        return videoItem;
    }
}