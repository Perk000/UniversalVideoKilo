using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VideoSelectionManager : MonoBehaviour
{
    [Header("Source Buttons")]
    public Button youTubeButton;
    public Button cameraRollButton;
    public Button databaseButton;
    public Button favoritesButton;

    [Header("Gallery Navigation")]
    public Button informationButton;
    public Button youtubeGalleryButton;
    public Button userVideosGalleryButton;

    [Header("Favorites Popup")]
    public GameObject favoritesPopup;
    public Transform favoritesContent; // Content of ScrollView
    public Button closePopupButton;
    public GameObject videoItemPrefab; // Prefab with Button and Text for each favorite (create manually)

    private VideoData videoData;
    private SceneManagerMenu sceneManager;

    void Awake()
    {
        if (favoritesPopup) favoritesPopup.SetActive(false);
    }

    void Start()
    {
        videoData = FindObjectOfType<VideoData>();
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        if (youTubeButton) youTubeButton.onClick.AddListener(OnYouTubeClicked);
        if (cameraRollButton) cameraRollButton.onClick.AddListener(OnCameraRollClicked);
        if (databaseButton) databaseButton.onClick.AddListener(OnDatabaseClicked);
        if (favoritesButton) favoritesButton.onClick.AddListener(OnFavoritesClicked);
        if (closePopupButton) closePopupButton.onClick.AddListener(CloseFavoritesPopup);

        // Gallery navigation buttons
        if (informationButton) informationButton.onClick.AddListener(OnInformationClicked);
        if (youtubeGalleryButton) youtubeGalleryButton.onClick.AddListener(OnYouTubeGalleryClicked);
        if (userVideosGalleryButton) userVideosGalleryButton.onClick.AddListener(OnUserVideosGalleryClicked);
    }

    void OnYouTubeClicked()
    {
        // Load YouTube input scene or show input field
        // For now, load a placeholder YouTube video
        string source = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"; // Placeholder
        sceneManager.LoadArVideoPlayer(source);
    }

    void OnInformationClicked()
    {
        if (sceneManager) sceneManager.LoadInformationPage();
    }

    void OnYouTubeGalleryClicked()
    {
        if (sceneManager) sceneManager.LoadYouTubeGallery();
    }

    void OnUserVideosGalleryClicked()
    {
        if (sceneManager) sceneManager.LoadUserVideosGallery();
    }

    void OnCameraRollClicked()
    {
        // Use DeviceVideoPickerTest for camera roll access
        GameObject pickerObj = GameObject.Find("VideoPickerManager");
        if (pickerObj != null)
        {
            pickerObj.SendMessage("OpenVideoPicker");
        }
        else
        {
            Debug.LogWarning("VideoPickerManager not found in scene");
        }
    }

    void OnDatabaseClicked()
    {
        // Use VideoStreamManager for database integration
        GameObject streamObj = GameObject.FindWithTag("VideoStreamManager") ?? GameObject.Find("VideoStreamManager");
        if (streamObj != null)
        {
            streamObj.SendMessage("LoadFromDatabase");
        }
        else
        {
            Debug.LogWarning("VideoStreamManager not found in scene");
        }
    }

    void OnFavoritesClicked()
    {
        LoadAndDisplayFavorites();
        favoritesPopup.SetActive(true);
    }

    void CloseFavoritesPopup()
    {
        favoritesPopup.SetActive(false);
    }

    void LoadAndDisplayFavorites()
    {
        // Clear existing items
        foreach (Transform child in favoritesContent)
        {
            Destroy(child.gameObject);
        }

        List<string> favorites = videoData.GetFavorites();
        foreach (string source in favorites)
        {
            GameObject item = Instantiate(videoItemPrefab, favoritesContent);
            TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
            if (text)
            {
                // Display source or a friendly name; here use source as is
                text.text = source;
            }
            Button itemButton = item.GetComponent<Button>();
            if (itemButton)
            {
                string sourceCopy = source; // Capture for lambda
                itemButton.onClick.AddListener(() => {
                    sceneManager.LoadArVideoPlayer(sourceCopy);
                    CloseFavoritesPopup();
                });
            }
        }
    }
}