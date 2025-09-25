using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SilverTau.NSR.Recorders.Clocks;
using SilverTau.NSR.Recorders.Inputs;
using SilverTau.NSR.Recorders.Internal;
using SilverTau.NSR.Recorders.Watermark;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Recorders.Video
{
    [AddComponentMenu("Silver Tau/NSR/Universal Video Recorder")]
    public class UniversalVideoRecorder : UVRecorder
    {
        public static UniversalVideoRecorder Instance { get; set; }
        
        #region Editor
        
#if UNITY_EDITOR
        public EditorEncoder editorEncoder = EditorEncoder.VP8;
        public VideoBitrateMode videoBitrateMode = VideoBitrateMode.Medium;
        
#if UNITY_2021_2_OR_NEWER
        public VideoEncodingProfile h264VideoEncodingProfile = VideoEncodingProfile.H264Main;
#endif
        public bool editorIncludeAlpha = false;

        private Color _editorCameraColor;
        
        private NSREditorCorder _nsrEditorCorder;
#endif
        
        #endregion
        
        #region MonoBehaviour

        private void Awake()
        {
            Instance = this;
        }

        public override void Start()
        {
            base.Start();
        }
        
        public override void Update()
        {
            base.Update();
        }

        #endregion

        #region Video Recorder
        
        /// <summary>
        /// A method that start recording.
        /// </summary>
        public void StartVideoRecorder(string videoFilePath = null, string videoFileName = null, string audioFilePath = null, string audioFileName = null)
        {
            if(IsRecording) return;

#if NSR_MICROPHONE_DISABLE
            recordMicrophone = false;
#endif
            
#if UNITY_EDITOR
            StartEditorVideoRecorder();
#else
            if (recordMicrophone)
            {
                StartCoroutine(PrepareMicrophone(error =>
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.Log(error);
                        return;
                    }
                    
                    StartRecorder(videoFilePath, videoFileName, audioFilePath, audioFileName);
                }));

                return;
            }

            StartRecorder(videoFilePath, videoFileName, audioFilePath, audioFileName);
#endif
        }

        public void StartVideoRecorder() => StartVideoRecorder(null, null, null, null);

        private IEnumerator PrepareMicrophone(Action<string> callback)
        {
            if (!recordMicrophone)
            {
                callback?.Invoke(null);
                yield break;
            }

            var error = string.Empty;
            
            MicrophoneSource = gameObject.GetComponent<AudioSource>();

            if (MicrophoneSource == null)
            {
                MicrophoneSource = gameObject.AddComponent<AudioSource>();
            }
        
            MicrophoneSource.mute = MicrophoneSource.loop = true;
            MicrophoneSource.bypassEffects = MicrophoneSource.bypassListenerEffects = false;

#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    Permission.RequestUserPermission(Permission.Microphone);
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(0.2f);
                }
#endif

#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            try
            {
                MicrophoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            }
            catch (Exception e)
            {
                error = e.ToString();
                callback?.Invoke(e.ToString());
            }

            if (!string.IsNullOrEmpty(error)) yield break;
                
            //_microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            if (Microphone.GetPosition(null) == 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
            callback?.Invoke(null);
#else
            callback?.Invoke("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
#endif
            
            yield return null;
        }
        
        /// <summary>
        /// Coroutine that check Permissions.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckPermissions()
        {
            
#if !UNITY_EDITOR
            if (recordMicrophone){
#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    Permission.RequestUserPermission(Permission.Microphone);
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(0.2f);
                }
#endif
            }
#endif
            yield break;
        }
        
        /// <summary>
        /// A method that stops recording.
        /// </summary>
        public async void StopVideoRecorder()
        {
            if(!IsRecording) return;
            
#if UNITY_EDITOR
            StopEditorVideoRecorder();
#else
            StopRecorder();

#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            if (recordMicrophone)
            {
                try
                {
                    Microphone.End(null);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
#else
            Debug.Log("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
#endif
#endif
            await Task.Delay(1000);
        }

        public void PauseVideoRecorder()
        {
#if UNITY_EDITOR
            PauseEditorVideoRecorder();
#else
            PauseRecorder();
#endif
        }

        public void ResumeVideoRecorder()
        {
#if UNITY_EDITOR
            ResumeEditorVideoRecorder();
#else
            ResumeRecorder();
#endif
        }
        
        #endregion
        
        #region Editor Corder

        private void StartEditorVideoRecorder()
        {
#if UNITY_EDITOR
            if (_nsrEditorCorder != null)
            {
                Destroy(_nsrEditorCorder);
            }

            _editorCameraColor = mainCamera.backgroundColor;
            
            _nsrEditorCorder = gameObject.AddComponent<NSREditorCorder>();
            _nsrEditorCorder.TargetCamera = mainCamera;
            _nsrEditorCorder.Cameras = cameras.Count > 0 ? cameras : new List<Camera> {mainCamera};
            _nsrEditorCorder.FileName = CustomOutputVideoFileName;
            _nsrEditorCorder.OutputVideoFilePath = CustomOutputVideoFilePath;
            _nsrEditorCorder.TargetFrameRate = frameRate;
            _nsrEditorCorder.HDR = HDR;
            _nsrEditorCorder.UseRenderTexture = useRenderTexture;
            _nsrEditorCorder.TargetRenderTexture = targetRenderTexture;
            _nsrEditorCorder.VideoResolution = GetTargetVideoResolution(videoResolution);
            _nsrEditorCorder.AudioReceiverMixer = audioReceiverMixer;
            _nsrEditorCorder.AudioReceivers = audioReceivers;
            
            _nsrEditorCorder.TargetEditorEncoder = editorEncoder;
            
#if UNITY_2021_2_OR_NEWER
            _nsrEditorCorder.TargetH264VideoEncodingProfile = h264VideoEncodingProfile;
#endif
            _nsrEditorCorder.TargetVideoBitrateMode = videoBitrateMode;
            _nsrEditorCorder.TargetIncludeAlpha = editorIncludeAlpha;
            
            _nsrEditorCorder.FrameDescriptorColorFormat = frameDescriptorColorFormat;
            _nsrEditorCorder.UseFrameDescriptorSrgb = useFrameDescriptorSrgb;
            _nsrEditorCorder.FrameDescriptorMsaaSamples = frameDescriptorMsaaSamples;

            _nsrEditorCorder.OnFrameRender = onFrameRender;
            
            if (editorIncludeAlpha)
            {
                mainCamera.backgroundColor = new Color(_editorCameraColor.r, _editorCameraColor.g, _editorCameraColor.b, 0.0f);
            }
            
            if (recordAllAudioSources)
            {
                _nsrEditorCorder.TargetAudioListener = audioListener;
            }
            else if (recordOnlyOneAudioSource)
            {
                _nsrEditorCorder.TargetAudioSource = targetAudioSource;
            }
            else
            {
                _nsrEditorCorder.TargetAudioListener = audioListener;
            }
            
            _nsrEditorCorder.StartRecorder();
            
            GetRecordStatus = RecordStatus.Recording;
            IsRecording = true;
            onStartCapture?.Invoke();
#endif
        }

        private void StopEditorVideoRecorder()
        {
#if UNITY_EDITOR
            if (_nsrEditorCorder == null) { return; }
            _nsrEditorCorder.StopRecorder();
            VideoOutputPath = _nsrEditorCorder.OutputVideoPath;
            mainCamera.backgroundColor = _editorCameraColor;
            
            GetRecordStatus = RecordStatus.Stopped;
            IsRecording = false;
            onStopCapture?.Invoke();
            
            onCompleteCapture?.Invoke();
#endif
        }

        private void PauseEditorVideoRecorder()
        {
#if UNITY_EDITOR
            if (!IsRecording) return;
            if (_nsrEditorCorder == null) { return; }
            _nsrEditorCorder.PauseRecorder();
            GetRecordStatus = RecordStatus.Paused;
            onPauseCapture?.Invoke();
#endif
        }

        private void ResumeEditorVideoRecorder()
        {
#if UNITY_EDITOR
            if (!IsRecording) return;
            if (_nsrEditorCorder == null) { return; }
            _nsrEditorCorder.ResumeRecorder();
            GetRecordStatus = RecordStatus.Recording;
            onResumeCapture?.Invoke();
#endif
        }

        #endregion
        
        #region Help Methods
        
        /// <summary>
        /// A method that converts RenderTexture and returns Texture2D.
        /// </summary>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        /// <param name="callback">Callback action.</param>
        public Texture2D CreatePreviewImage(int width = 0, int height = 0, Action callback = null)
        {
            if (width <= 0) width = Screen.width;
            if (height <= 0) height = Screen.height;
            
            var renderTexture = !targetRenderTexture ? new RenderTexture(width, height, 24, HDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32) : targetRenderTexture;
            var rect = new Rect(0, 0, renderTexture.width, renderTexture.height);

            var isLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;
            var previewImage = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, isLinear);
            
            if (!targetRenderTexture)
            {
                mainCamera.targetTexture = renderTexture;
                mainCamera.Render();
            }
 
            RenderTexture.active = renderTexture;
            previewImage.ReadPixels(rect, 0, 0);
            previewImage.Apply();
            
            if (!targetRenderTexture)
            {
                mainCamera.targetTexture = null;
                Destroy(renderTexture);
                renderTexture = null;
            }
 
            RenderTexture.active = null;
            
            callback?.Invoke();

            return previewImage;
        }
        
        #endregion
    }
}