using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class VideoPreviewHandheld : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        [SerializeField] private GameObject canvas;
        
        [Space(10)]
        
        [Header("UI")]
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Button buttonShare;
        [SerializeField] private Button buttonSaveToGallery;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonPreview;
        
        // Stores the path to the video file.
        private string _videoPath;
        
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

            if (rawImage)
            {
                if (rawImage.texture != null)
                {
                    Destroy(rawImage.texture);
                }
                
                rawImage.texture = universalVideoRecorder.CreatePreviewImage();
            }
            
            if (canvas) canvas.SetActive(true);
        }

        private void OnDestroy()
        {
            if (rawImage)
            {
                if (rawImage.texture != null)
                {
                    Destroy(rawImage.texture);
                }
            }
        }

        private void Back()
        {
            universalVideoRecorder.Dispose();
            if (canvas) canvas.SetActive(false);
            _videoPath = string.Empty;
        }

        private void OnSaveVideoToGallery(bool result, string error)
        {
            Debug.Log(result);
            Debug.Log(error);
        }

        private void Preview()
        {
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(_videoPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            UniversalVideoRecorder.Instance.Preview();
#endif
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
            Utilities.Gallery.SaveVideoToGallery(_videoPath);
#endif
        }
    }
}
