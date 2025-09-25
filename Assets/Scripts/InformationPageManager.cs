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

        // Setup container sizes for mobile optimization
        SetupContainerSizes();
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
            GameObject stepItem = CreateStepItem();
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
        // Create compact feature item for mobile screens
        GameObject featureItem = new GameObject("FeatureItem");
        RectTransform rectTransform = featureItem.AddComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Failed to create RectTransform for feature item");
            Destroy(featureItem);
            return null;
        }

        // Smaller height for mobile
        rectTransform.sizeDelta = new Vector2(0, 90);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        featureItem.AddComponent<CanvasRenderer>();
        Image background = featureItem.AddComponent<Image>();
        if (background != null)
        {
            background.color = new Color(1f, 1f, 1f, 0.05f); // More subtle background
        }

        // Icon (smaller)
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(featureItem.transform);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        if (iconRect == null)
        {
            Debug.LogError("Failed to create RectTransform for feature icon");
            Destroy(featureItem);
            return null;
        }

        // Smaller icon area
        iconRect.anchorMin = new Vector2(0, 0.2f);
        iconRect.anchorMax = new Vector2(0.18f, 0.9f);
        iconRect.offsetMin = new Vector2(5, 0);
        iconRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        if (iconText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for feature icon");
            Destroy(featureItem);
            return null;
        }

        iconText.fontSize = 24; // Smaller icon
        iconText.alignment = TextAlignmentOptions.Center;

        // Title (compact)
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(featureItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        if (titleRect == null)
        {
            Debug.LogError("Failed to create RectTransform for feature title");
            Destroy(featureItem);
            return null;
        }

        // Adjusted for smaller icon
        titleRect.anchorMin = new Vector2(0.22f, 0.4f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(5, 0);
        titleRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        if (titleText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for feature title");
            Destroy(featureItem);
            return null;
        }

        titleText.fontSize = 16; // Smaller title
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description (compact)
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(featureItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        if (descRect == null)
        {
            Debug.LogError("Failed to create RectTransform for feature description");
            Destroy(featureItem);
            return null;
        }

        // Full width for description
        descRect.anchorMin = new Vector2(0.22f, 0);
        descRect.anchorMax = new Vector2(1, 0.4f);
        descRect.offsetMin = new Vector2(5, 0);
        descRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        if (descText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for feature description");
            Destroy(featureItem);
            return null;
        }

        descText.fontSize = 12; // Smaller description
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // Add controller
        FeatureItemController controller = featureItem.AddComponent<FeatureItemController>();
        if (controller == null)
        {
            Debug.LogError("Failed to create FeatureItemController");
            Destroy(featureItem);
            return null;
        }

        controller.iconText = iconText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.backgroundImage = background;

        return featureItem;
    }

    GameObject CreateStepItem()
    {
        // Create a compact version optimized for mobile screens
        GameObject stepItem = new GameObject("StepItem");
        RectTransform rectTransform = stepItem.AddComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Failed to create RectTransform for step item");
            Destroy(stepItem);
            return null;
        }

        // Smaller height for mobile screens
        rectTransform.sizeDelta = new Vector2(0, 60);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add CanvasRenderer
        stepItem.AddComponent<CanvasRenderer>();

        // Step Number (compact version)
        GameObject numberObj = new GameObject("StepNumber");
        numberObj.transform.SetParent(stepItem.transform);
        RectTransform numberRect = numberObj.AddComponent<RectTransform>();
        if (numberRect == null)
        {
            Debug.LogError("Failed to create RectTransform for step number");
            Destroy(stepItem);
            return null;
        }

        // Smaller number area
        numberRect.anchorMin = new Vector2(0, 0.3f);
        numberRect.anchorMax = new Vector2(0.12f, 0.9f);
        numberRect.offsetMin = new Vector2(5, 0);
        numberRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI numberText = numberObj.AddComponent<TextMeshProUGUI>();
        if (numberText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for step number");
            Destroy(stepItem);
            return null;
        }

        numberText.text = "1";
        numberText.fontSize = 16;
        numberText.color = new Color(0.2f, 0.6f, 1f);
        numberText.alignment = TextAlignmentOptions.Center;
        numberText.fontStyle = FontStyles.Bold;

        // Title (compact)
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(stepItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        if (titleRect == null)
        {
            Debug.LogError("Failed to create RectTransform for step title");
            Destroy(stepItem);
            return null;
        }

        // Adjusted for smaller number area
        titleRect.anchorMin = new Vector2(0.15f, 0.4f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(5, 0);
        titleRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        if (titleText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for step title");
            Destroy(stepItem);
            return null;
        }

        titleText.fontSize = 14;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description (compact)
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(stepItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        if (descRect == null)
        {
            Debug.LogError("Failed to create RectTransform for step description");
            Destroy(stepItem);
            return null;
        }

        // Full width for description
        descRect.anchorMin = new Vector2(0.15f, 0);
        descRect.anchorMax = new Vector2(1, 0.4f);
        descRect.offsetMin = new Vector2(5, 0);
        descRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        if (descText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for step description");
            Destroy(stepItem);
            return null;
        }

        descText.fontSize = 12;
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // Simple line separator (compact)
        GameObject lineObj = new GameObject("Separator");
        lineObj.transform.SetParent(stepItem.transform);
        RectTransform lineRect = lineObj.AddComponent<RectTransform>();
        if (lineRect == null)
        {
            Debug.LogError("Failed to create RectTransform for separator");
            Destroy(stepItem);
            return null;
        }

        // Very thin separator at bottom
        lineRect.anchorMin = new Vector2(0, -0.05f);
        lineRect.anchorMax = new Vector2(1, 0);
        lineRect.offsetMin = new Vector2(10, 0);
        lineRect.offsetMax = new Vector2(-10, 1);

        TextMeshProUGUI separatorText = lineObj.AddComponent<TextMeshProUGUI>();
        if (separatorText == null)
        {
            Debug.LogError("Failed to create TextMeshProUGUI for separator");
            Destroy(stepItem);
            return null;
        }

        separatorText.text = new string('─', 30); // Shorter line
        separatorText.fontSize = 8;
        separatorText.color = new Color(0.9f, 0.9f, 0.9f);
        separatorText.alignment = TextAlignmentOptions.Center;

        // Add controller (without Image references)
        StepItemController controller = stepItem.AddComponent<StepItemController>();
        if (controller == null)
        {
            Debug.LogError("Failed to create StepItemController");
            Destroy(stepItem);
            return null;
        }

        controller.stepNumberText = numberText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.stepCircleImage = null;
        controller.connectorLineImage = null;

        return stepItem;
    }

    void SetupContainerSizes()
    {
        // Setup container sizes optimized for mobile screens
        if (featuresContainer)
        {
            RectTransform featuresRect = featuresContainer.GetComponent<RectTransform>();
            if (featuresRect)
            {
                // 6 features × 90 height each = 540 total height
                featuresRect.sizeDelta = new Vector2(335, 540);
            }
        }

        if (stepsContainer)
        {
            RectTransform stepsRect = stepsContainer.GetComponent<RectTransform>();
            if (stepsRect)
            {
                // 6 steps × 60 height each = 360 total height
                stepsRect.sizeDelta = new Vector2(335, 360);
            }
        }
    }
}