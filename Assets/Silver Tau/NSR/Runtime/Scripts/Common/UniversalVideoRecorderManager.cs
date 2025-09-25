using System;
using System.IO;
using SilverTau.NSR.Recorders.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Recorders.Video
{
    public class UniversalVideoRecorderManager : MonoBehaviour
    {
        public RawImage rawImage;
        [SerializeField] private Button buttonShare;
        [SerializeField] private Button buttonSaveToGallery;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonPreview;
        
        private ScreenOrientation _screenOrientation;
        private bool _autorotateToPortrait;
        private bool _autorotateToPortraitUpsideDown;
        private bool _autorotateToLandscapeLeft;
        private bool _autorotateToLandscapeRight;

        private void Start()
        {
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
            _screenOrientation = Screen.orientation;

            _autorotateToPortrait = Screen.autorotateToPortrait;
            _autorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
            _autorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft;
            _autorotateToLandscapeRight = Screen.autorotateToLandscapeRight;
            
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            if (rawImage) rawImage.texture = UniversalVideoRecorder.Instance.CreatePreviewImage();
            
            Utilities.Gallery.onSaveVideoToGallery += OnSaveVideoToGallery;
        }

        private void OnDisable()
        {
            Screen.autorotateToPortrait = _autorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = _autorotateToPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = _autorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = _autorotateToLandscapeRight;

            Utilities.Gallery.onSaveVideoToGallery -= OnSaveVideoToGallery;
        }

        private void OnDestroy()
        {
            Screen.autorotateToPortrait = _autorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = _autorotateToPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = _autorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = _autorotateToLandscapeRight;
        }

        private void Back()
        {
            UniversalVideoRecorder.Instance.Dispose();
        }

        private void OnSaveVideoToGallery(bool result, string error)
        {
            Debug.Log(result);
            Debug.Log(error);
        }
        
        private void SaveVideo()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
            if (string.IsNullOrEmpty(outputPath))
            {
                return;
            }
            
            //Save video
            //Use your own methods to save the video. The final recording path corresponds to the VideoOutputPath parameter.
            try
            {
                var resultVideoFileBytes = File.ReadAllBytes(outputPath);

                var format = string.Empty;
            
                switch (UniversalVideoRecorder.Instance.encodeTo)
                {
                    case EncodeTo.MP4:
                        format = ".mp4";
                        break;
                    default:
                        format = ".mp4";
                        break;
                }
            
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, NSRCoreUtility.CurrentOutputVideoFileName + format), resultVideoFileBytes);
            }
            catch (Exception e)
            {
                Debug.Log("Save error:" + e);
                return;
            }
        }

        private void Preview()
        {
#if PLATFORM_STANDALONE || UNITY_EDITOR
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(outputPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            UniversalVideoRecorder.Instance.Preview();
#endif
        }

        private void Share()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(outputPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            Utilities.Share.ShareItem(outputPath);
#endif
        }

        private void SaveToGallery()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(outputPath);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            Utilities.Gallery.SaveVideoToGallery(outputPath);
#endif
        }
    }
}
