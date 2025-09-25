using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class VideoItemController : MonoBehaviour
{
    [Header("UI Components")]
    public RawImage thumbnailImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI viewCountText;
    public TextMeshProUGUI durationText;
    public Button playARButton;
    public Button playFullScreenButton;
    public Image categoryBadge;
    public TextMeshProUGUI categoryText;

    private YouTubeVideoData videoData;
    private UserVideoData userVideoData;
    private SceneManagerMenu sceneManager;
    private bool isUserVideo = false;

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        if (playARButton)
        {
            playARButton.onClick.AddListener(OnPlayAR);
        }

        if (playFullScreenButton)
        {
            playFullScreenButton.onClick.AddListener(OnPlayFullScreen);
        }
    }

    public void SetupVideo(YouTubeVideoData data)
    {
        videoData = data;
        isUserVideo = false;

        if (titleText) titleText.text = data.title;
        if (descriptionText) descriptionText.text = data.description;
        if (viewCountText) viewCountText.text = FormatViewCount(data.viewCount);
        if (durationText) durationText.text = data.duration;
        if (categoryText) categoryText.text = data.category;

        // Load thumbnail
        if (!string.IsNullOrEmpty(data.thumbnailUrl))
        {
            StartCoroutine(LoadThumbnail(data.thumbnailUrl));
        }

        // Set category badge color
        if (categoryBadge)
        {
            categoryBadge.color = GetCategoryColor(data.category);
        }
    }

    private IEnumerator LoadThumbnail(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            if (thumbnailImage && texture != null)
            {
                thumbnailImage.texture = texture;
            }
        }
        else
        {
            Debug.LogWarning($"Failed to load thumbnail from {url}: {request.error}");
            // Optionally set a default texture or placeholder
        }

        // Dispose of the request to prevent memory leaks
        request.Dispose();
    }

    private string FormatViewCount(string viewCount)
    {
        if (long.TryParse(viewCount, out long views))
        {
            if (views >= 1000000)
            {
                return $"{views / 1000000.0f:F1}M views";
            }
            else if (views >= 1000)
            {
                return $"{views / 1000.0f:F1}K views";
            }
            else
            {
                return $"{views} views";
            }
        }
        return viewCount;
    }

    private Color GetCategoryColor(string category)
    {
        switch (category.ToLower())
        {
            case "tutorials":
                return new Color(0.2f, 0.6f, 1.0f); // Blue
            case "entertainment":
                return new Color(1.0f, 0.4f, 0.0f); // Orange
            case "educational":
                return new Color(0.0f, 0.8f, 0.0f); // Green
            case "music":
                return new Color(0.8f, 0.0f, 0.8f); // Purple
            default:
                return new Color(0.5f, 0.5f, 0.5f); // Gray
        }
    }

    private void OnPlayAR()
    {
        if (sceneManager != null)
        {
            string source;
            if (isUserVideo && userVideoData != null)
            {
                // Store user video data
                PlayerPrefs.SetString("selectedVideoTitle", userVideoData.title);
                PlayerPrefs.SetString("selectedVideoSource", userVideoData.localPath);
                source = userVideoData.localPath;
            }
            else if (videoData != null)
            {
                // Store YouTube video data
                PlayerPrefs.SetString("selectedVideoTitle", videoData.title);
                PlayerPrefs.SetString("selectedVideoId", videoData.videoId);
                source = videoData.videoId; // Use videoId as direct URL for testing
            }
            else
            {
                return;
            }

            // Load AR video player
            sceneManager.LoadArVideoPlayer(source);
        }
    }

    private void OnPlayFullScreen()
    {
        if (sceneManager != null)
        {
            string source;
            if (isUserVideo && userVideoData != null)
            {
                // Store user video data
                PlayerPrefs.SetString("selectedVideoTitle", userVideoData.title);
                PlayerPrefs.SetString("selectedVideoSource", userVideoData.localPath);
                source = userVideoData.localPath;
            }
            else if (videoData != null)
            {
                // Store YouTube video data
                PlayerPrefs.SetString("selectedVideoTitle", videoData.title);
                PlayerPrefs.SetString("selectedVideoId", videoData.videoId);
                source = videoData.videoId; // Use videoId as direct URL for testing
            }
            else
            {
                return;
            }

            // Load full screen video player
            sceneManager.LoadFullScreenVideoPlayer(source);
        }
    }

    // Alternative setup method for user videos
    public void SetupUserVideo(UserVideoData data)
    {
        userVideoData = data;
        isUserVideo = true;

        if (titleText) titleText.text = data.title;
        if (descriptionText) descriptionText.text = data.description;
        if (viewCountText) viewCountText.text = $"{data.viewCount} views";
        if (durationText) durationText.text = data.duration;

        // For user videos, use local thumbnail or generate one
        if (thumbnailImage && data.thumbnailTexture)
        {
            thumbnailImage.texture = data.thumbnailTexture;
        }

        // Hide category elements for user videos
        if (categoryBadge) categoryBadge.gameObject.SetActive(false);
        if (categoryText) categoryText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class UserVideoData
{
    public string title;
    public string description;
    public int viewCount;
    public string duration;
    public Texture2D thumbnailTexture;
    public string localPath;
}