using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIPrefabFactory
{
    // Create a VideoItem prefab at runtime
    public static GameObject CreateVideoItem()
    {
        GameObject videoItem = new GameObject("VideoItem");
        RectTransform rectTransform = videoItem.AddComponent<RectTransform>();

        // Setup RectTransform
        rectTransform.sizeDelta = new Vector2(160, 200);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add components
        videoItem.AddComponent<CanvasRenderer>();
        Image background = videoItem.AddComponent<Image>();
        background.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Thumbnail (RawImage)
        GameObject thumbnailObj = new GameObject("Thumbnail");
        thumbnailObj.transform.SetParent(videoItem.transform);
        RectTransform thumbRect = thumbnailObj.AddComponent<RectTransform>();
        thumbRect.anchorMin = new Vector2(0, 0.6f);
        thumbRect.anchorMax = new Vector2(1, 1);
        thumbRect.offsetMin = new Vector2(0, 0);
        thumbRect.offsetMax = new Vector2(0, 0);

        RawImage thumbnail = thumbnailObj.AddComponent<RawImage>();
        thumbnail.color = Color.white;

        // Title Text
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

        // Description Text
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

        // View Count Text
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

        // Duration Text
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

        // Add VideoItemController script
        VideoItemController controller = videoItem.AddComponent<VideoItemController>();
        controller.thumbnailImage = thumbnail;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.viewCountText = viewText;
        controller.durationText = durationText;
        controller.playButton = playButton;

        return videoItem;
    }

    // Create a FeatureItem prefab at runtime
    public static GameObject CreateFeatureItem()
    {
        GameObject featureItem = new GameObject("FeatureItem");
        RectTransform rectTransform = featureItem.AddComponent<RectTransform>();

        // Setup RectTransform
        rectTransform.sizeDelta = new Vector2(0, 120);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add components
        featureItem.AddComponent<CanvasRenderer>();
        Image background = featureItem.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.1f);

        // Icon Text
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(featureItem.transform);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0);
        iconRect.anchorMax = new Vector2(0.2f, 1);
        iconRect.offsetMin = new Vector2(0, 0);
        iconRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.text = "🎥";
        iconText.fontSize = 32;
        iconText.color = new Color(0.2f, 0.6f, 1f);
        iconText.alignment = TextAlignmentOptions.Center;

        // Title Text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(featureItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.25f, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(0, 0);
        titleRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Feature Title";
        titleText.fontSize = 18;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description Text
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(featureItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.25f, 0);
        descRect.anchorMax = new Vector2(1, 0.5f);
        descRect.offsetMin = new Vector2(0, 0);
        descRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Feature description with detailed information about what this feature does.";
        descText.fontSize = 14;
        descText.color = new Color(0.5f, 0.5f, 0.5f);
        descText.alignment = TextAlignmentOptions.TopLeft;

        // Add FeatureItemController script
        FeatureItemController controller = featureItem.AddComponent<FeatureItemController>();
        controller.iconText = iconText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.backgroundImage = background;

        return featureItem;
    }

    // Create a StepItem prefab at runtime
    public static GameObject CreateStepItem()
    {
        GameObject stepItem = new GameObject("StepItem");
        RectTransform rectTransform = stepItem.AddComponent<RectTransform>();

        // Setup RectTransform
        rectTransform.sizeDelta = new Vector2(0, 80);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add components
        stepItem.AddComponent<CanvasRenderer>();

        // Step Number Text
        GameObject numberObj = new GameObject("StepNumber");
        numberObj.transform.SetParent(stepItem.transform);
        RectTransform numberRect = numberObj.AddComponent<RectTransform>();
        numberRect.anchorMin = new Vector2(0, 0);
        numberRect.anchorMax = new Vector2(0.15f, 1);
        numberRect.offsetMin = new Vector2(0, 0);
        numberRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI numberText = numberObj.AddComponent<TextMeshProUGUI>();
        numberText.text = "1";
        numberText.fontSize = 16;
        numberText.color = Color.white;
        numberText.alignment = TextAlignmentOptions.Center;

        // Step Circle (background for number)
        Image stepCircle = numberObj.AddComponent<Image>();
        stepCircle.color = new Color(0.2f, 0.6f, 1f);

        // Title Text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(stepItem.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.2f, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(0, 0);
        titleRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Step Title";
        titleText.fontSize = 16;
        titleText.color = new Color(0.2f, 0.2f, 0.2f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.fontStyle = FontStyles.Bold;

        // Description Text
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(stepItem.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.2f, 0);
        descRect.anchorMax = new Vector2(1, 0.5f);
        descRect.offsetMin = new Vector2(0, 0);
        descRect.offsetMax = new Vector2(-10, 0);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Step description with detailed instructions.";
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

        // Add StepItemController script
        StepItemController controller = stepItem.AddComponent<StepItemController>();
        controller.stepNumberText = numberText;
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.stepCircleImage = stepCircle;
        controller.connectorLineImage = connectorLine;

        return stepItem;
    }
}