using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SilverTau.NSR.Samples
{
    public class VideoPreviewPlayer : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        [SerializeField] private GameObject canvas;
        
        [Space(10)]
        
        [Header("UI")]
        [SerializeField] private RawImage rawImage;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        [SerializeField] private Button buttonShare;
        [SerializeField] private Button buttonSaveToGallery;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonPreview;
        [SerializeField] private Text textPreview;
        
        [Space(10)]
        
        [Header("Android")]
#pragma warning disable CS0414 // Field is assigned but its value is never used
        [SerializeField] private string galleryFolderPath = "NSR_Video";
#pragma warning restore CS0414 // Field is assigned but its value is never used
        
        // Holds the last created video player GameObject.
        private GameObject _lastVideoPlayerObject;
        
        // Reference to the VideoPlayer component.
        private VideoPlayer _videoPlayer;
        private RenderTexture _renderTexture;
        
        // Stores the path to the video file.
        private string _videoPath;
        
        private bool _playerStatus;
        private bool _isPlayerPrepareCompleted;
        
        private void Start()
        {
            if (canvas) canvas.SetActive(false);

            if (buttonBack)
            {
                buttonBack.onClick.AddListener(Back);
#if PLATFORM_STANDALONE
                if(buttonBack) buttonBack.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if(buttonBack) buttonBack.gameObject.SetActive(true);
#endif
            }

            if (buttonShare)
            {
                buttonShare.onClick.AddListener(Share);
#if PLATFORM_STANDALONE
                if(buttonShare) buttonShare.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if(buttonShare) buttonShare.gameObject.SetActive(true);
#endif
            }

            if (buttonSaveToGallery)
            {
                buttonSaveToGallery.onClick.AddListener(SaveToGallery);
#if PLATFORM_STANDALONE
                if(buttonSaveToGallery) buttonSaveToGallery.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if(buttonSaveToGallery) buttonSaveToGallery.gameObject.SetActive(true);
#endif
            }

            if (buttonPreview)
            {
                buttonPreview.onClick.AddListener(Preview);
#if PLATFORM_STANDALONE
                if(buttonPreview) buttonPreview.gameObject.SetActive(false);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if(buttonPreview) buttonPreview.gameObject.SetActive(true);
#endif
            }
        }
        
        private void OnEnable()
        {
            // Subscribe to the onCompleteCapture event of the UniversalVideoRecorder instance.
            // This event triggers when the video capture is complete.
            universalVideoRecorder.onCompleteCapture += OnCompleteCapture;
            Utilities.Gallery.onSaveVideoToGallery += OnSaveVideoToGallery;
        }

        private void OnDisable()
        {
            universalVideoRecorder.onCompleteCapture -= OnCompleteCapture;
            Utilities.Gallery.onSaveVideoToGallery -= OnSaveVideoToGallery;
        }

        /// <summary>
        /// This method is called when the video capture is complete.
        /// </summary>
        private void OnCompleteCapture()
        {
            // Retrieve the output path of the recorded video.
            _videoPath = universalVideoRecorder.VideoOutputPath;
            
            if (canvas) canvas.SetActive(true);
            
            // Play the recorded video.
            Prepare();
        }
        
        /// <summary>
        /// Method to prepare the video from the recorded file path.
        /// </summary>
        private void Prepare()
        {
            // Destroy the previous video player GameObject if it exists.
            if (_lastVideoPlayerObject != null)
            {
                Destroy(_lastVideoPlayerObject);
            }

            // Create a new GameObject to host the VideoPlayer component.
            _lastVideoPlayerObject = new GameObject("VideoPlayer");

            // Add a VideoPlayer component to the new GameObject.
            _videoPlayer = _lastVideoPlayerObject.AddComponent<VideoPlayer>();

            // Configure the VideoPlayer:
            _videoPlayer.playOnAwake = false; // Do not play the video immediately upon creation.
            _videoPlayer.isLooping = true; // Loop the video playback.

            // Set the video source to a URL and provide the file path.
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = _videoPath;

            var width = Screen.width;
            var height = Screen.height;

            if (universalVideoRecorder.targetRenderTexture != null)
            {
                width = universalVideoRecorder.targetRenderTexture.width;
                height = universalVideoRecorder.targetRenderTexture.height;
            }
            
            _renderTexture = new RenderTexture(width, height, 24, GraphicsFormat.R8G8B8A8_UNorm);

            aspectRatioFitter.aspectRatio = (float)width / (float)height;
            
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            if (rawImage) rawImage.texture = _renderTexture;

            // Prepare the video (e.g., buffer it), then play when preparation is complete.
            _videoPlayer.Prepare();
            _videoPlayer.prepareCompleted += (source) =>
            {
                _isPlayerPrepareCompleted = true;
                _videoPlayer.Play();
                _videoPlayer.Pause();
            };

            _videoPlayer.errorReceived += (vp, message) =>
            {
                Debug.LogError($"Video Player Error: {message}");
            };
        }

        private void Back()
        {
            universalVideoRecorder.Dispose();
            if (canvas) canvas.SetActive(false);
            
            // Destroy the video player GameObject if it exists.
            if (_lastVideoPlayerObject != null)
            {
                Destroy(_lastVideoPlayerObject);
                _lastVideoPlayerObject = null;
            }

            if (_renderTexture != null)
            {
                Destroy(_renderTexture);
            }

            _videoPath = string.Empty;

            _isPlayerPrepareCompleted = false;
            _playerStatus = false;
            if (textPreview) textPreview.text = "Play";
        }

        private void OnSaveVideoToGallery(bool result, string error)
        {
            Debug.Log(result);
            Debug.Log(error);
        }

        private void Preview()
        {
            if (!_isPlayerPrepareCompleted) return;
            
            if (_playerStatus)
            {
                _videoPlayer.Pause();
            }
            else
            {
                _videoPlayer.Play();
            }
            
            _playerStatus = !_playerStatus;
            if (textPreview) textPreview.text = _playerStatus ? "Pause" : "Play";
        }

        private void Share()
        {
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(_videoPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            Utilities.Share.ShareItem(_videoPath);
#endif
        }

        private void SaveToGallery()
        {
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(_videoPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            Utilities.Gallery.SaveVideoToGallery(_videoPath, galleryFolderPath);
#endif
        }
    }
}
