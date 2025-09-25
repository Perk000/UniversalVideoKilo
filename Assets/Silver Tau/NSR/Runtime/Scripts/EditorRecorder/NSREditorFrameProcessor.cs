using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Media;
using Unity.Collections;
#endif

namespace SilverTau.NSR.Recorders
{
    public class NSREditorFrameProcessor 
    {
#if UNITY_EDITOR
        public int TargetWidth { get; private set; }
        public int TargetHeight { get; private set; }
        
        public RenderTexture TargetRenderTexture { get; set; }
        public RenderTextureFormat FrameDescriptorColorFormat { get; set; } = RenderTextureFormat.ARGB32;
        public bool UseFrameDescriptorSrgb { get; set; } = true;
        public int FrameDescriptorMsaaSamples { get; set; } = 1;
        
        private RenderTextureDescriptor frameDescriptor;
        
        private Texture2D _tempTexture2D;
        private Rect _rect;
        private bool _initialized = false;
        
        public bool IsRecording { get; set; }
        public bool IsPaused { get; set; }
        
#if UNITY_2021_2_OR_NEWER
        public VideoTrackEncoderAttributes VideoAttributes { get; private set; }
#else
		public VideoTrackAttributes VideoAttributes { get; private set; }
#endif

        public void Initialize(int width, int height, bool hdr, RenderTexture targetRenderTexture = null, RenderTextureFormat frameDescriptorColorFormat = RenderTextureFormat.ARGB32, bool useFrameDescriptorSrgb = true, int frameDescriptorMsaaSamples = 1)
        {
            TargetRenderTexture = targetRenderTexture;
            
            FrameDescriptorColorFormat = frameDescriptorColorFormat;
            UseFrameDescriptorSrgb = useFrameDescriptorSrgb;
            FrameDescriptorMsaaSamples = frameDescriptorMsaaSamples;
            
			if (_tempTexture2D != null)
			{
				UnityEngine.Object.DestroyImmediate(_tempTexture2D);
			}
            TargetWidth = TargetRenderTexture ? TargetRenderTexture.width : width;
            TargetHeight = TargetRenderTexture ? TargetRenderTexture.height : height;
            _rect = new Rect(0, 0, TargetWidth, TargetHeight);
            
            if (TargetRenderTexture == null)
            {
                TargetRenderTexture = new RenderTexture(TargetWidth, TargetHeight, 24, RenderTextureFormat.ARGB32);
            }
            
            _tempTexture2D = new Texture2D(TargetWidth, TargetHeight, TextureFormat.RGBA32, false);
            
            frameDescriptor = new RenderTextureDescriptor(TargetWidth, TargetHeight, RenderTextureFormat.ARGB32, 24) {
                sRGB = true,
                msaaSamples = Mathf.Max(QualitySettings.antiAliasing, 1)
            };

            frameDescriptor.colorFormat = FrameDescriptorColorFormat;
            frameDescriptor.sRGB = UseFrameDescriptorSrgb;
            frameDescriptor.msaaSamples = FrameDescriptorMsaaSamples;
            
            _initialized = true;
        }

        
#if UNITY_2021_2_OR_NEWER
        public void InitEncoder(int frameRate, EditorEncoder encoder, VideoBitrateMode bitrateMode, VideoEncodingProfile videoH264EncodingProfile, bool includeAlpha, out string fileFormat)
#else
        public void InitEncoder(int frameRate, EditorEncoder encoder, VideoBitrateMode bitrateMode, bool includeAlpha, out string fileFormat)
#endif
        {
            fileFormat = ".mp4";
			
#if UNITY_2021_2_OR_NEWER
            switch (encoder)
            {
                case EditorEncoder.H264:
                    var h264Attr = new H264EncoderAttributes
                    {
                        gopSize = 25,
                        numConsecutiveBFrames = 2,
                        profile = videoH264EncodingProfile
                    };
					
                    SetupVideoAttributes(new VideoTrackEncoderAttributes(h264Attr)
                    {
                        frameRate = new MediaRational(frameRate),
                        width = (uint)TargetWidth,
                        height = (uint)TargetHeight,
                        includeAlpha = includeAlpha,
                        bitRateMode = bitrateMode
                    });
					
                    fileFormat = ".mp4";
                    break;
                case EditorEncoder.VP8:
                    var vp8Attr = new VP8EncoderAttributes
                    {
                        keyframeDistance = 25
                    };
					
                    SetupVideoAttributes(new VideoTrackEncoderAttributes(vp8Attr)
                    {
                        frameRate = new MediaRational(frameRate),
                        width = (uint)TargetWidth,
                        height = (uint)TargetHeight,
                        includeAlpha = includeAlpha,
                        bitRateMode = bitrateMode
                    });
                    fileFormat = ".webm";
                    break;
                default:
                    break;
            }
#else
			SetupVideoAttributes(new VideoTrackAttributes
            {
                frameRate = new MediaRational(frameRate),
                width = (uint)TargetWidth,
                height = (uint)TargetHeight,
#if UNITY_2018_1_OR_NEWER
                includeAlpha = includeAlpha,
                bitRateMode = bitrateMode
#endif
            });

			fileFormat = ".mp4";
#endif
        }
        
#if UNITY_2021_2_OR_NEWER
        public void SetupVideoAttributes(VideoTrackEncoderAttributes attributes)
        {
            VideoAttributes = attributes;
        }
#else
        public void SetupVideoAttributes(VideoTrackAttributes attributes)
        {
            VideoAttributes = attributes;
        }
#endif

        public void CaptureFrameSync(MediaEncoder mediaEncoder, Camera camera, UnityAction<Texture> onFrameRender)
        {
            if (!_initialized) return;
            if (!IsRecording) return;
            if (IsPaused) return;
            if (!CheckVideoSize()) return;
            
            RenderTexture.active = TargetRenderTexture;
            if (TargetRenderTexture != camera.targetTexture)
            {
                var prevTarget = camera.targetTexture;
                camera.targetTexture = TargetRenderTexture;
                camera.Render();
                _tempTexture2D.ReadPixels(_rect, 0, 0);
                camera.targetTexture = prevTarget;
            }
            else
            {
                _tempTexture2D.ReadPixels(_rect, 0, 0);
            }
            onFrameRender?.Invoke(_tempTexture2D);
            mediaEncoder?.AddFrame(_tempTexture2D);
            RenderTexture.active = null;
        }

        public void CaptureFrameAsync(MediaEncoder mediaEncoder, Camera camera, UnityAction<Texture> onFrameRender)
        {
            if (!_initialized) return;
            if (!IsRecording) return;
            if (IsPaused) return;
            if (!CheckVideoSize()) return;
            
            var tempRT = RenderTexture.GetTemporary(frameDescriptor);
            //var tempRT = RenderTexture.GetTemporary(TargetWidth, TargetHeight, 24, RenderTextureFormat.ARGB32);
            
            if (TargetRenderTexture != camera.targetTexture)
            {
                var prevTarget = camera.targetTexture;
                camera.targetTexture = TargetRenderTexture;
                camera.Render();
                camera.targetTexture = prevTarget;
            }
            Graphics.Blit(TargetRenderTexture, tempRT);
            onFrameRender?.Invoke(tempRT);
            
            AsyncGPUReadback.Request(tempRT, 0, request =>
            {
                if (!IsRecording) return;
                if (IsPaused) return;
                if (_tempTexture2D == null) return;
                var pixels32 = request.GetData<Color32>();
                _tempTexture2D.SetPixels32(pixels32.ToArray());
                mediaEncoder?.AddFrame(_tempTexture2D);
            });
            RenderTexture.ReleaseTemporary(tempRT);
        }

        public void Dispose()
        {
            if (_tempTexture2D != null)
            {
                UnityEngine.Object.DestroyImmediate(_tempTexture2D);
            }

            _initialized = false;
            IsRecording = false;
            IsPaused = false;
        }

        private bool CheckVideoSize()
        {
            if((!TargetRenderTexture ? Screen.width : TargetRenderTexture.width) != TargetWidth || (!TargetRenderTexture ? Screen.height : TargetRenderTexture.height) != TargetHeight)
            {
                Debug.LogWarning("Render window size has changed!");
                return false;
            }
            return true;
        }
#endif
    }
}