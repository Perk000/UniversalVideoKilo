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
    public Transform categoryContainer; // Container for dynamic category buttons
    public GameObject categoryButtonPrefab; // Prefab for category buttons

    [Header("Video Items")]
    public GameObject videoItemPrefab; // Prefab for video items

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
    private List<Button> categoryButtons = new List<Button>();
    private List<string> categories = new List<string>();

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        // Setup UI events
        if (searchButton) searchButton.onClick.AddListener(OnSearch);
        if (backButton) backButton.onClick.AddListener(OnBack);
        if (informationButton) informationButton.onClick.AddListener(OnInformation);
        if (userVideosButton) userVideosButton.onClick.AddListener(OnUserVideos);

        // Setup search input
        if (searchInput)
        {
            searchInput.onEndEdit.AddListener(OnSearchInputEndEdit);
        }

        // Load initial videos (which also loads categories)
        LoadYouTubeVideos();
    }

    void LoadYouTubeVideos()
    {
        ShowLoading("Loading YouTube videos...");

        // Clear existing videos
        ClearVideoGrid();

        // Simulate loading videos (replace with actual YouTube API call)
        videoList = GetMockYouTubeVideos();

        // Load categories from videos
        LoadCategories();

        // Display videos
        DisplayVideos(videoList);

        HideLoading();
    }

    void LoadCategories()
    {
        // Clear existing category buttons
        foreach (Transform child in categoryContainer)
        {
            Destroy(child.gameObject);
        }
        categoryButtons.Clear();

        // Get categories from videos (or from API/database later)
        categories = GetAvailableCategories();

        // Create "All" button first
        CreateCategoryButton("All", "all");

        // Create buttons for each category
        foreach (string category in categories)
        {
            CreateCategoryButton(CapitalizeFirst(category), category.ToLower());
        }
    }

    void CreateCategoryButton(string displayName, string categoryValue)
    {
        if (categoryButtonPrefab == null) return;

        GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (text) text.text = displayName;
        if (button)
        {
            button.onClick.AddListener(() => FilterByCategory(categoryValue));
            categoryButtons.Add(button);
        }
    }

    List<string> GetAvailableCategories()
    {
        // Extract unique categories from video list
        HashSet<string> uniqueCategories = new HashSet<string>();
        foreach (YouTubeVideoData video in videoList)
        {
            if (!string.IsNullOrEmpty(video.category))
            {
                uniqueCategories.Add(video.category);
            }
        }
        return new List<string>(uniqueCategories);
    }

    string CapitalizeFirst(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1);
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
                // Use runtime creation for reliable layout
                GameObject videoItem = CreateVideoItem();
                videoItem.transform.SetParent(videoGridContainer, false);
                VideoItemController controller = videoItem.GetComponent<VideoItemController>();

                if (controller)
                {
                    controller.SetupVideo(video);
                }
            }
        }

        // Force layout rebuild to position items correctly
        LayoutRebuilder.ForceRebuildLayoutImmediate(videoGridContainer as RectTransform);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(videoGridContainer as RectTransform);

        // Debug: Log positions of instantiated items
        foreach (Transform child in videoGridContainer)
        {
            Debug.Log($"VideoItem {child.name} position: {child.GetComponent<RectTransform>().anchoredPosition}");
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
        ColorBlock normalColors = new ColorBlock();
        normalColors.normalColor = Color.white;
        normalColors.selectedColor = Color.white;
        normalColors.colorMultiplier = 1f;
        normalColors.fadeDuration = 0.1f;

        ColorBlock selectedColors = new ColorBlock();
        selectedColors.normalColor = new Color(0.2f, 0.6f, 1.0f);
        selectedColors.selectedColor = new Color(0.2f, 0.6f, 1.0f);
        selectedColors.colorMultiplier = 1f;
        selectedColors.fadeDuration = 0.1f;

        for (int i = 0; i < categoryButtons.Count; i++)
        {
            Button button = categoryButtons[i];
            string categoryValue = (i == 0) ? "all" : categories[i - 1].ToLower(); // First is "All", then categories
            button.colors = (currentCategory == categoryValue) ? selectedColors : normalColors;
        }
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
                videoId = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4", // Test with direct video URL
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
        rectTransform.anchoredPosition = Vector2.zero; // Reset position for layout
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


        // Play AR Button
        GameObject arButtonObj = new GameObject("PlayARButton");
        arButtonObj.transform.SetParent(videoItem.transform);
        RectTransform arButtonRect = arButtonObj.AddComponent<RectTransform>();
        arButtonRect.anchorMin = new Vector2(0, 0);
        arButtonRect.anchorMax = new Vector2(0.5f, 0.1f);
        arButtonRect.offsetMin = new Vector2(0, 0);
        arButtonRect.offsetMax = new Vector2(0, 0);

        Button playARButton = arButtonObj.AddComponent<Button>();
        Image arButtonImage = arButtonObj.AddComponent<Image>();
        arButtonImage.color = new Color(0.2f, 0.6f, 1f);

        // Add text to AR button
        GameObject arTextObj = new GameObject("ARText");
        arTextObj.transform.SetParent(arButtonObj.transform);
        RectTransform arTextRect = arTextObj.AddComponent<RectTransform>();
        arTextRect.anchorMin = new Vector2(0, 0);
        arTextRect.anchorMax = new Vector2(1, 1);
        arTextRect.offsetMin = new Vector2(0, 0);
        arTextRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI arButtonText = arTextObj.AddComponent<TextMeshProUGUI>();
        arButtonText.text = "AR";
        arButtonText.fontSize = 12;
        arButtonText.color = Color.white;
        arButtonText.alignment = TextAlignmentOptions.Center;

        // Play Full Screen Button
        GameObject fsButtonObj = new GameObject("PlayFullScreenButton");
        fsButtonObj.transform.SetParent(videoItem.transform);
        RectTransform fsButtonRect = fsButtonObj.AddComponent<RectTransform>();
        fsButtonRect.anchorMin = new Vector2(0.5f, 0);
        fsButtonRect.anchorMax = new Vector2(1, 0.1f);
        fsButtonRect.offsetMin = new Vector2(0, 0);
        fsButtonRect.offsetMax = new Vector2(0, 0);

        Button playFullScreenButton = fsButtonObj.AddComponent<Button>();
        Image fsButtonImage = fsButtonObj.AddComponent<Image>();
        fsButtonImage.color = new Color(0.6f, 0.2f, 1f);

        // Add text to Full Screen button
        GameObject fsTextObj = new GameObject("FSText");
        fsTextObj.transform.SetParent(fsButtonObj.transform);
        RectTransform fsTextRect = fsTextObj.AddComponent<RectTransform>();
        fsTextRect.anchorMin = new Vector2(0, 0);
        fsTextRect.anchorMax = new Vector2(1, 1);
        fsTextRect.offsetMin = new Vector2(0, 0);
        fsTextRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI fsButtonText = fsTextObj.AddComponent<TextMeshProUGUI>();
        fsButtonText.text = "Full";
        fsButtonText.fontSize = 12;
        fsButtonText.color = Color.white;
        fsButtonText.alignment = TextAlignmentOptions.Center;

        // Add controller
        VideoItemController controller = videoItem.AddComponent<VideoItemController>();
        controller.thumbnailImage = thumbnail;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.viewCountText = viewText;
        controller.durationText = durationText;
        controller.playARButton = playARButton;
        controller.playFullScreenButton = playFullScreenButton;

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