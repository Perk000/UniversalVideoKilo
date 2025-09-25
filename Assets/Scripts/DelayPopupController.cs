using UnityEngine;
using UnityEngine.UI;

public class DelayPopupController : MonoBehaviour
{
    public GameObject popupPanel;
    public Button delay5Button;
    public Button delay10Button;
    public Button delay15Button;
    public Button cancelButton;

    private ArVideoPlayerManager videoManager;

    void Awake()
    {
        HidePopup();
    }

    public void ShowPopup()
    {
        if (popupPanel) popupPanel.SetActive(true);

        videoManager = FindObjectOfType<ArVideoPlayerManager>();

        if (delay5Button) delay5Button.onClick.AddListener(() => SelectDelay(5));
        if (delay10Button) delay10Button.onClick.AddListener(() => SelectDelay(10));
        if (delay15Button) delay15Button.onClick.AddListener(() => SelectDelay(15));
        if (cancelButton) cancelButton.onClick.AddListener(HidePopup);
    }

    public void HidePopup()
    {
        if (popupPanel) popupPanel.SetActive(false);
    }

    private void SelectDelay(float delay)
    {
        if (videoManager)
        {
            videoManager.DelayedPlay(delay, this);
        }
    }
}