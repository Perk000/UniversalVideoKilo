using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepItemController : MonoBehaviour
{
    public TextMeshProUGUI stepNumberText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image stepCircleImage;
    public Image connectorLineImage;

    public void SetupStep(int stepNumber, string title, string description)
    {
        if (stepNumberText) stepNumberText.text = stepNumber.ToString();
        if (titleText) titleText.text = title;
        if (descriptionText) descriptionText.text = description;

        // Style the step circle
        if (stepCircleImage)
        {
            stepCircleImage.color = GetStepColor(stepNumber);
        }

        // Hide connector line for last step (optional)
        if (connectorLineImage && stepNumber == 6) // Assuming 6 steps total
        {
            connectorLineImage.enabled = false;
        }
    }

    private Color GetStepColor(int stepNumber)
    {
        // Return different colors for step progression
        float hue = (stepNumber - 1) / 6.0f; // 6 steps total
        return Color.HSVToRGB(hue, 0.7f, 0.9f);
    }
}