using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeatureItemController : MonoBehaviour
{
    public TextMeshProUGUI iconText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image backgroundImage;

    public void SetupFeature(string icon, string title, string description)
    {
        if (iconText) iconText.text = icon;
        if (titleText) titleText.text = title;
        if (descriptionText) descriptionText.text = description;

        // Optional: Set background color based on feature type
        if (backgroundImage)
        {
            // You can customize colors based on the feature
            Color backgroundColor = GetFeatureColor(title);
            backgroundImage.color = backgroundColor;
        }
    }

    private Color GetFeatureColor(string featureTitle)
    {
        // Return different colors for different feature types
        switch (featureTitle)
        {
            case "AR Video Playback":
                return new Color(0.2f, 0.6f, 1.0f, 0.1f); // Blue
            case "YouTube Integration":
                return new Color(1.0f, 0.0f, 0.0f, 0.1f); // Red
            case "Camera Roll Access":
                return new Color(0.0f, 0.8f, 0.0f, 0.1f); // Green
            case "Screen Recording":
                return new Color(0.8f, 0.4f, 0.0f, 0.1f); // Orange
            case "Social Sharing":
                return new Color(0.6f, 0.2f, 0.8f, 0.1f); // Purple
            case "Offline Viewing":
                return new Color(0.4f, 0.4f, 0.4f, 0.1f); // Gray
            default:
                return new Color(1.0f, 1.0f, 1.0f, 0.1f); // White
        }
    }
}