using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class ArVideoPlayerManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private string videoSource;
    public AudioSource audioSource;
    public AudioClip chimeClip;

    void Start()
    {
        videoSource = PlayerPrefs.GetString("videoSource", "");
        Debug.Log("ArVideoPlayerManager: videoSource = " + videoSource);
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("ArVideoPlayerManager: VideoPlayer component not found!");
            return;
        }

        // Ensure VideoPlayer is configured correctly
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.None; // Disable audio for now
        videoPlayer.targetMaterialProperty = "_MainTex"; // Ensure correct material property
        videoPlayer.isLooping = true; // Loop the video until paused
        videoPlayer.playbackSpeed = 1f; // Ensure normal speed
        videoPlayer.skipOnDrop = true; // Skip frames if lagging to maintain smoothness
        videoPlayer.waitForFirstFrame = false; // Start playback immediately for smoothness
        videoPlayer.timeUpdateMode = UnityEngine.Video.VideoTimeUpdateMode.GameTime; // Use game time for consistent playback
        Debug.Log("ArVideoPlayerManager: VideoPlayer renderMode = " + videoPlayer.renderMode);
        Debug.Log("ArVideoPlayerManager: VideoPlayer source = " + videoPlayer.source);
        Debug.Log("ArVideoPlayerManager: VideoPlayer targetMaterialProperty = " + videoPlayer.targetMaterialProperty);

        // Check renderer and material
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            // Create material instance to avoid shared material issues
            renderer.material = Instantiate(renderer.material);
            Debug.Log("ArVideoPlayerManager: Renderer found, material = " + renderer.material.name);
            Debug.Log("ArVideoPlayerManager: Material shader = " + renderer.material.shader.name);
        }
        else
        {
            Debug.LogError("ArVideoPlayerManager: Renderer component not found!");
        }

        if (!string.IsNullOrEmpty(videoSource))
        {
            // Ensure URL has file:// prefix for local files
            string finalUrl = videoSource;
            if (!videoSource.StartsWith("http") && !videoSource.StartsWith("file://"))
            {
                finalUrl = "file://" + videoSource;
            }
            videoPlayer.url = finalUrl;
            Debug.Log("ArVideoPlayerManager: Setting video URL to " + finalUrl);
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            Debug.LogWarning("ArVideoPlayerManager: No video source set in PlayerPrefs");
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("ArVideoPlayerManager: Video prepared, starting playback");
        vp.Play();
        Debug.Log("ArVideoPlayerManager: Started playing video");

        // Check if texture is applied to material
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Texture texture = renderer.material.GetTexture("_MainTex");
            Debug.Log("ArVideoPlayerManager: Material _MainTex = " + (texture != null ? texture.name : "null"));
        }
    }

    public void PlayPause()
    {
        Debug.Log("PlayPause: videoPlayer.isPlaying = " + videoPlayer.isPlaying);
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Paused video");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("Played video");
        }
    }

    public void DelayedPlay(float delay, DelayPopupController popup = null)
    {
        StartCoroutine(DelayedPlayCoroutine(delay, popup));
    }

    private IEnumerator DelayedPlayCoroutine(float delay, DelayPopupController popup = null)
    {
        TextMeshProUGUI countdownText = GameObject.Find("CountdownText")?.GetComponent<TextMeshProUGUI>();
        if (countdownText) countdownText.text = "";

        float remainingTime = delay;
        if (countdownText) countdownText.text = remainingTime.ToString();
        while (remainingTime > 5f)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
            if (countdownText) countdownText.text = remainingTime.ToString();
        }

        // Now countdown the last 5 seconds with chimes
        for (int i = 5; i > 0; i--)
        {
            if (countdownText) countdownText.text = i.ToString();
            if (audioSource && chimeClip)
            {
                audioSource.PlayOneShot(chimeClip);
            }
            yield return new WaitForSeconds(1f);
        }

        // Play the video
        PlayVideo();
        if (countdownText) countdownText.text = "";
        if (popup) popup.HidePopup();
    }

    private void PlayVideo()
    {
        if (videoPlayer)
        {
            videoPlayer.Play();
        }
    }

    public void SetOpacity(float opacity)
    {
        // Assuming the material is transparent
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer)
        {
            Color color = renderer.material.GetColor("_BaseColor");
            color.a = opacity;
            renderer.material.SetColor("_BaseColor", color);
        }
    }

    public void SetTimeline(float value)
    {
        if (videoPlayer)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    public void ResetVideo()
    {
        if (videoPlayer)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
    }

    public string GetVideoName()
    {
        return System.IO.Path.GetFileName(videoSource);
    }

    public string GetTimeClock()
    {
        if (videoPlayer)
        {
            return $"{videoPlayer.time:F2} / {videoPlayer.length:F2}";
        }
        return "0.00 / 0.00";
    }
}