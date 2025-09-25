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
    public Button playButton;
    public Image categoryBadge;
    public TextMeshProUGUI categoryText;

    private YouTubeVideoData videoData;
    private SceneManagerMenu sceneManager;

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerMenu>();

        if (playButton)
        {
            playButton.onClick.AddListener(OnPlayVideo);
        }
    }

    public void SetupVideo(YouTubeVideoData data)
    {
        videoData = data;

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
            if (thumbnailImage)
            {
                thumbnailImage.texture = texture;
            }
        }
        else
        {
            Debug.LogError($"Failed to load thumbnail: {request.error}");
        }
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

    private void OnPlayVideo()
    {
        if (videoData != null && sceneManager != null)
        {
            // Store video data for playback
            PlayerPrefs.SetString("selectedVideoTitle", videoData.title);
            PlayerPrefs.SetString("selectedVideoId", videoData.videoId);

            // Load AR video player with YouTube URL
            string youtubeUrl = $"https://www.youtube.com/watch?v={videoData.videoId}";
            sceneManager.LoadArVideoPlayer(youtubeUrl);
        }
    }

    // Alternative setup method for user videos
    public void SetupUserVideo(UserVideoData data)
    {
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