using UnityEngine;
using UnityEngine.Video;

public class ArVideoPlayerManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private string videoSource;

    void Start()
    {
        videoSource = PlayerPrefs.GetString("videoSource", "");
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer && !string.IsNullOrEmpty(videoSource))
        {
            videoPlayer.url = videoSource;
            videoPlayer.Play();
        }
    }

    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    public void SetOpacity(float opacity)
    {
        // Assuming the material is transparent
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            Color color = renderer.material.color;
            color.a = opacity;
            renderer.material.color = color;
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