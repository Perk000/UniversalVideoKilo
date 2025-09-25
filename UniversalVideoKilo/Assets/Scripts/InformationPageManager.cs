using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InformationPageManager : MonoBehaviour
{
    [Header("Hero Section")]
    public TextMeshProUGUI heroTitle;
    public TextMeshProUGUI heroSubtitle;
    public Button getStartedButton;
    public Button exploreGalleriesButton;

    [Header("Features Section")]
    public Transform featuresContainer;

    [Header("Getting Started Section")]
    public Transform stepsContainer;

    [Header("Navigation")]
    public Button backToMenuButton;
    public Button youtubeGalleryButton;
    public Button userVideosButton;

    private SceneManagerMenu sceneManager;

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        // Setup hero section
        if (heroTitle) heroTitle.text = "AR Video Player";
        if (heroSubtitle) heroSubtitle.text = "Experience videos in augmented reality with advanced playback controls and social features";

        // Setup buttons
        if (getStartedButton) getStartedButton.onClick.AddListener(OnGetStarted);
        if (exploreGalleriesButton) exploreGalleriesButton.onClick.AddListener(OnExploreGalleries);
        if (backToMenuButton) backToMenuButton.onClick.AddListener(OnBackToMenu);
        if (youtubeGalleryButton) youtubeGalleryButton.onClick.AddListener(OnYouTubeGallery);
        if (userVideosButton) userVideosButton.onClick.AddListener(OnUserVideos);

        // Initialize content
        InitializeFeatures();
        InitializeSteps();
    }

    void InitializeFeatures()
    {
        if (!featuresContainer) return;

        // Clear existing items
        foreach (Transform child in featuresContainer)
        {
            Destroy(child.gameObject);
        }

        // Feature data
        string[] featureTitles = {
            "AR Video Playback",
            "YouTube Integration",
            "Camera Roll Access",
            "Screen Recording",
            "Social Sharing",
            "Offline Viewing"
        };

        string[] featureDescriptions = {
            "Play videos in augmented reality with precise positioning and scaling",
            "Stream YouTube videos directly with advanced controls",
            "Access and play videos from your device camera roll",
            "Record your screen with professional quality",
            "Share videos with friends and view engagement metrics",
            "Download videos for offline viewing and playback"
        };

        string[] featureIcons = { "🎥", "📺", "📱", "🎬", "📤", "💾" };

        for (int i = 0; i < featureTitles.Length; i++)
        {
            // Create feature item manually
            GameObject featureItem = CreateFeatureItem();
            featureItem.transform.SetParent(featuresContainer, false);
            FeatureItemController controller = featureItem.GetComponent<FeatureItemController>();

            if (controller)
            {
                controller.SetupFeature(featureIcons[i], featureTitles[i], featureDescriptions[i]);
            }
        }
    }

    void InitializeSteps()
    {
        if (!stepsContainer) return;

        // Clear existing items
        foreach (Transform child in stepsContainer)
        {
            Destroy(child.gameObject);
        }

        // Step data
        string[] stepTitles = {
            "Launch the App",
            "Choose Video Source",
            "Position in AR",
            "Control Playback",
            "Explore Galleries",
            "Share & Record"
        };

        string[] stepDescriptions = {
            "Open the AR Video Player app on your iPhone",
            "Select from YouTube, Camera Roll, or Database sources",
            "Point your camera at a surface to place the video",
            "Use intuitive controls to play, pause, and seek",
            "Browse curated video galleries and user content",
            "Record your own videos and share with others"
        };

        for (int i = 0; i < stepTitles.Length; i++)
        {
            GameObject stepItem = UIPrefabFactory.CreateStepItem();
            stepItem.transform.SetParent(stepsContainer, false);
            StepItemController controller = stepItem.GetComponent<StepItemController>();

            if (controller)
            {
                controller.SetupStep(i + 1, stepTitles[i], stepDescriptions[i]);
            }
        }
    }

    // Button event handlers
    void OnGetStarted()
    {
        if (sceneManager) sceneManager.LoadVideoSelection();
    }

    void OnExploreGalleries()
    {
        if (sceneManager) sceneManager.LoadYouTubeGallery();
    }

    void OnBackToMenu()
    {
        if (sceneManager) sceneManager.LoadMainMenu();
    }

    void OnYouTubeGallery()
    {
        if (sceneManager) sceneManager.LoadYouTubeGallery();
    }

    void OnUserVideos()
    {
        if (sceneManager) sceneManager.LoadUserVideosGallery();
    }

    // Helper methods to create UI components
    GameObject CreateFeatureItem()
    {
        GameObject featureItem = new GameObject("FeatureItem");
        RectTransform rectTransform = featureItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 120);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        featureItem.AddComponent<CanvasRenderer>();
        Image background = featureItem.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.1f);

        // Icon
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(featureItem.transform);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0);
        iconRect.anchorMax = new Vector2(0.2f, 1);
        iconRect.offsetMin = new Vector2(0, 0);
        iconRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 32;
        iconText.alignment = TextAlignmentOptions.Center;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(featureItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.25f, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(0, 0);
        titleRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 18;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(featureItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.25f, 0);
        descRect.anchorMax = new Vector2(1, 0.5f);
        descRect.offsetMin = new Vector2(0, 0);
        descRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 14;
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // Add controller
        FeatureItemController controller = featureItem.AddComponent<FeatureItemController>();
        controller.iconText = iconText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.backgroundImage = background;

        return featureItem;
    }

    GameObject CreateStepItem()
    {
        GameObject stepItem = new GameObject("StepItem");
        RectTransform rectTransform = stepItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 80);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        stepItem.AddComponent<CanvasRenderer>();

        // Step Number
        GameObject numberObj = new GameObject("StepNumber");
        numberObj.transform.SetParent(stepItem.transform);
        RectTransform numberRect = numberObj.AddComponent<RectTransform>();
        numberRect.anchorMin = new Vector2(0, 0);
        numberRect.anchorMax = new Vector2(0.15f, 1);
        numberRect.offsetMin = new Vector2(0, 0);
        numberRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI numberText = numberObj.AddComponent<TextMeshProUGUI>();
        numberText.fontSize = 16;
        numberText.color = Color.white;
        numberText.alignment = TextAlignmentOptions.Center;

        Image stepCircle = numberObj.AddComponent<Image>();
        stepCircle.color = new Color(0.2f, 0.6f, 1f);

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(stepItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.2f, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(0, 0);
        titleRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 16;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(stepItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.2f, 0);
        descRect.anchorMax = new Vector2(1, 0.5f);
        descRect.offsetMin = new Vector2(0, 0);
        descRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 14;
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // Connector Line
        GameObject lineObj = new GameObject("ConnectorLine");
        lineObj.transform.SetParent(stepItem.transform);
        RectTransform lineRect = lineObj.AddComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.1f, -0.5f);
        lineRect.anchorMax = new Vector2(0.1f, 0);
        lineRect.offsetMin = new Vector2(0, 0);
        lineRect.offsetMax = new Vector2(2, 0);

        Image connectorLine = lineObj.AddComponent<Image>();
        connectorLine.color = new Color(0.8f, 0.8f, 0.8f);

        // Add controller
        StepItemController controller = stepItem.AddComponent<StepItemController>();
        controller.stepNumberText = numberText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.stepCircleImage = stepCircle;
        controller.connectorLineImage = connectorLine;

        return stepItem;
    }
}