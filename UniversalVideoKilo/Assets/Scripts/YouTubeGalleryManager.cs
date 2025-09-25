using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class YouTubeGalleryManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField searchInput;
    public Button searchButton;
    public ScrollRect galleryScrollRect;
    public Transform videoGridContainer;

    [Header("Category Filters")]
    public Button allButton;
    public Button tutorialsButton;
    public Button entertainmentButton;
    public Button educationalButton;
    public Button musicButton;

    [Header("Navigation")]
    public Button backButton;
    public Button informationButton;
    public Button userVideosButton;

    [Header("Loading")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;

    private SceneManagerMenu sceneManager;
    private string currentCategory = "all";
    private List<YouTubeVideoData> videoList = new List<YouTubeVideoData>();

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        // Setup UI events
        if (searchButton) searchButton.onClick.AddListener(OnSearch);
        if (backButton) backButton.onClick.AddListener(OnBack);
        if (informationButton) informationButton.onClick.AddListener(OnInformation);
        if (userVideosButton) userVideosButton.onClick.AddListener(OnUserVideos);

        // Setup category buttons
        if (allButton) allButton.onClick.AddListener(() => FilterByCategory("all"));
        if (tutorialsButton) tutorialsButton.onClick.AddListener(() => FilterByCategory("tutorials"));
        if (entertainmentButton) entertainmentButton.onClick.AddListener(() => FilterByCategory("entertainment"));
        if (educationalButton) educationalButton.onClick.AddListener(() => FilterByCategory("educational"));
        if (musicButton) musicButton.onClick.AddListener(() => FilterByCategory("music"));

        // Setup search input
        if (searchInput)
        {
            searchInput.onEndEdit.AddListener(OnSearchInputEndEdit);
        }

        // Load initial videos
        LoadYouTubeVideos();
    }

    void LoadYouTubeVideos()
    {
        ShowLoading("Loading YouTube videos...");

        // Clear existing videos
        ClearVideoGrid();

        // Simulate loading videos (replace with actual YouTube API call)
        videoList = GetMockYouTubeVideos();

        // Display videos
        DisplayVideos(videoList);

        HideLoading();
    }

    void ClearVideoGrid()
    {
        foreach (Transform child in videoGridContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayVideos(List<YouTubeVideoData> videos)
    {
        foreach (YouTubeVideoData video in videos)
        {
            if (ShouldDisplayVideo(video))
            {
                GameObject videoItem = CreateVideoItem();
                videoItem.transform.SetParent(videoGridContainer, false);
                VideoItemController controller = videoItem.GetComponent<VideoItemController>();

                if (controller)
                {
                    controller.SetupVideo(video);
                }
            }
        }
    }

    bool ShouldDisplayVideo(YouTubeVideoData video)
    {
        if (currentCategory == "all") return true;

        // Filter by category
        return video.category.ToLower() == currentCategory.ToLower();
    }

    void FilterByCategory(string category)
    {
        currentCategory = category;
        DisplayVideos(videoList);

        // Update button states
        UpdateCategoryButtonStates();
    }

    void UpdateCategoryButtonStates()
    {
        // Reset all buttons
        ColorBlock normalColors = allButton.colors;
        normalColors.normalColor = Color.white;
        normalColors.selectedColor = Color.white;

        ColorBlock selectedColors = allButton.colors;
        selectedColors.normalColor = new Color(0.2f, 0.6f, 1.0f);
        selectedColors.selectedColor = new Color(0.2f, 0.6f, 1.0f);

        if (allButton) allButton.colors = (currentCategory == "all") ? selectedColors : normalColors;
        if (tutorialsButton) tutorialsButton.colors = (currentCategory == "tutorials") ? selectedColors : normalColors;
        if (entertainmentButton) entertainmentButton.colors = (currentCategory == "entertainment") ? selectedColors : normalColors;
        if (educationalButton) educationalButton.colors = (currentCategory == "educational") ? selectedColors : normalColors;
        if (musicButton) musicButton.colors = (currentCategory == "music") ? selectedColors : normalColors;
    }

    void OnSearch()
    {
        string query = searchInput.text.Trim();
        if (!string.IsNullOrEmpty(query))
        {
            SearchYouTubeVideos(query);
        }
    }

    void OnSearchInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSearch();
        }
    }

    void SearchYouTubeVideos(string query)
    {
        ShowLoading($"Searching for '{query}'...");

        // Simulate search (replace with actual YouTube API search)
        List<YouTubeVideoData> searchResults = videoList.FindAll(v =>
            v.title.ToLower().Contains(query.ToLower()) ||
            v.description.ToLower().Contains(query.ToLower()));

        ClearVideoGrid();
        DisplayVideos(searchResults);

        HideLoading();
    }

    void ShowLoading(string message)
    {
        if (loadingPanel) loadingPanel.SetActive(true);
        if (loadingText) loadingText.text = message;
    }

    void HideLoading()
    {
        if (loadingPanel) loadingPanel.SetActive(false);
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

    void OnUserVideos()
    {
        if (sceneManager) sceneManager.LoadUserVideosGallery();
    }

    // Mock data for demonstration
    List<YouTubeVideoData> GetMockYouTubeVideos()
    {
        return new List<YouTubeVideoData>
        {
            new YouTubeVideoData {
                title = "AR Video Player Tutorial",
                description = "Learn how to use AR video playback features",
                thumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/0.jpg",
                videoId = "dQw4w9WgXcQ",
                viewCount = "125000",
                duration = "5:30",
                category = "tutorials"
            },
            new YouTubeVideoData {
                title = "Amazing AR Experiences",
                description = "See incredible augmented reality demonstrations",
                thumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/0.jpg",
                videoId = "dQw4w9WgXcQ",
                viewCount = "2500000",
                duration = "8:15",
                category = "entertainment"
            },
            new YouTubeVideoData {
                title = "Unity AR Foundation Guide",
                description = "Complete guide to AR development in Unity",
                thumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/0.jpg",
                videoId = "dQw4w9WgXcQ",
                viewCount = "500000",
                duration = "15:20",
                category = "educational"
            },
            new YouTubeVideoData {
                title = "AR Music Visualizer",
                description = "Experience music through augmented reality",
                thumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/0.jpg",
                videoId = "dQw4w9WgXcQ",
                viewCount = "750000",
                duration = "6:45",
                category = "music"
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

[System.Serializable]
public class YouTubeVideoData
{
    public string title;
    public string description;
    public string thumbnailUrl;
    public string videoId;
    public string viewCount;
    public string duration;
    public string category;
}