using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor.Media;
using SilverTau.NSR.Recorders.Audio;
#endif
using System.IO;
using SilverTau.NSR.Recorders.Video;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace SilverTau.NSR.Recorders
{
	[Serializable]
	public enum EditorEncoder
	{
		H264 = 0,
		VP8 = 1
	}
	
	public class NSREditorCorder : MonoBehaviour
	{
#if UNITY_EDITOR
		public Camera TargetCamera { get; set; }
		public List<Camera> Cameras { get; set; } = new List<Camera>();
		public AudioListener TargetAudioListener { get; set; }
		public AudioSource TargetAudioSource { get; set; }
		public int TargetFrameRate { get; set; } = 30;
		public Vector2 VideoResolution { get; set; } = new Vector2(1920, 1080);
		
#if UNITY_EDITOR
		public EditorEncoder TargetEditorEncoder { get; set; } = EditorEncoder.VP8;
		public VideoBitrateMode TargetVideoBitrateMode { get; set; } = VideoBitrateMode.Medium;
		
#if UNITY_2021_2_OR_NEWER
		public VideoEncodingProfile TargetH264VideoEncodingProfile { get; set; } = VideoEncodingProfile.H264Main;
#endif
		public bool TargetIncludeAlpha { get; set; } = false;
#endif
		
		public string FileName { get; set; }
		public string OutputVideoFilePath { get; set; }
		public bool HDR { get; set; }
		public bool AudioReceiverMixer { get; set; }
		public bool UseRenderTexture { get; set; }
		public RenderTexture TargetRenderTexture { get; set; }
		
		public UnityAction<Texture> OnFrameRender { get; set; }
		
		public string OutputVideoPath { get; set; }

		private bool _isRecording;
		private bool _isPaused;
		private string _encodedFilePath;
		
		//FrameDescriptor - RenderTexture Settings
		public RenderTextureFormat FrameDescriptorColorFormat { get; set; } = RenderTextureFormat.ARGB32;
		public bool UseFrameDescriptorSrgb { get; set; } = true;
		public int FrameDescriptorMsaaSamples { get; set; } = 1;
		
#if UNITY_EDITOR
		public List<NSRAudioReceiver> AudioReceivers { get; set; } = new List<NSRAudioReceiver>();
		private MediaEncoder _mediaEncoder;
#endif
		private float timeAccumulator = 0f;
		
		private NSREditorFrameProcessor _nsrEditorFrameProcessor = new NSREditorFrameProcessor();
		private NSREditorAudioProcessor _nsrEditorAudioProcessor = new NSREditorAudioProcessor();
		
		private void Start () { }
		
		private void Update()
		{
#if UNITY_EDITOR
			if (!_isRecording || _isPaused || _mediaEncoder == null) return;
			if (_nsrEditorFrameProcessor == null) return;
			
			timeAccumulator += Time.deltaTime;
			float frameInterval = 1.0f / TargetFrameRate;
			
			while (timeAccumulator >= frameInterval)
			{
				foreach (var camera in Cameras)
				{
					if (camera == null || !camera.enabled || !camera.gameObject.activeSelf) continue;

					if (SystemInfo.supportsAsyncGPUReadback)
					{
						_nsrEditorFrameProcessor.CaptureFrameAsync(_mediaEncoder, camera, OnFrameRender);
					}
					else
					{
						_nsrEditorFrameProcessor.CaptureFrameSync(_mediaEncoder, camera, OnFrameRender);
					}
				}
				
				_nsrEditorAudioProcessor.CaptureAudio(_mediaEncoder);
				timeAccumulator -= frameInterval;
			}
#endif
		}
		
		private void PrepareRecorder()	
		{
#if UNITY_EDITOR
			if (TargetRenderTexture != null)
			{
				if(!UseRenderTexture && TargetRenderTexture != TargetCamera.targetTexture) DestroyImmediate(TargetRenderTexture);
			}
			
			_nsrEditorFrameProcessor.Initialize((int)VideoResolution.x, (int)VideoResolution.y, HDR, TargetRenderTexture, FrameDescriptorColorFormat, UseFrameDescriptorSrgb, FrameDescriptorMsaaSamples);
			
#if UNITY_2021_2_OR_NEWER
			_nsrEditorFrameProcessor.InitEncoder(TargetFrameRate, TargetEditorEncoder, TargetVideoBitrateMode, TargetH264VideoEncodingProfile, TargetIncludeAlpha, out var fileFormat);
#else
			_nsrEditorFrameProcessor.InitEncoder(TargetFrameRate, TargetEditorEncoder, TargetVideoBitrateMode, TargetIncludeAlpha, out var fileFormat);
#endif
			
			_nsrEditorAudioProcessor.Initialize(TargetFrameRate, TargetAudioListener, TargetAudioSource, AudioReceivers);
			_nsrEditorAudioProcessor.SetupAudioAttributes(TargetFrameRate);
			
			timeAccumulator = 0f;
			
			_encodedFilePath = GetEncodedFilePath(fileFormat);
			
			_mediaEncoder = new MediaEncoder(_encodedFilePath, _nsrEditorFrameProcessor.VideoAttributes, _nsrEditorAudioProcessor.AudioAttributes);
#endif
		}

		public void StartRecorder()
		{
#if UNITY_EDITOR
			PrepareRecorder();
			_isRecording = true;
			if (_nsrEditorFrameProcessor != null) _nsrEditorFrameProcessor.IsRecording = true;
#endif
		}

		public void StopRecorder()
		{
#if UNITY_EDITOR
			OutputVideoPath = _encodedFilePath;
			
			_mediaEncoder?.Dispose();
			_nsrEditorFrameProcessor?.Dispose();
			_nsrEditorAudioProcessor?.Dispose();
			
			timeAccumulator = 0f;
			
			_isRecording = false;
			_isPaused = false;
#endif
		}

		public void PauseRecorder()
		{
			_isPaused = true;
			if (_nsrEditorFrameProcessor != null) _nsrEditorFrameProcessor.IsPaused = true;
		}

		public void ResumeRecorder()
		{
			_isPaused = false;
			if (_nsrEditorFrameProcessor != null) _nsrEditorFrameProcessor.IsPaused = false;
		}

		private string GetEncodedFilePath(string fileFormat)
		{
			if (string.IsNullOrEmpty(FileName))
			{
				var timeFormat = "yyyy_MM_dd_HH_mm_ss_fff";
				FileName = DateTime.Now.ToString(timeFormat);
			}

			if (string.IsNullOrEmpty(OutputVideoFilePath))
			{
				OutputVideoFilePath = Path.Combine(Application.persistentDataPath);
			}
			
			_encodedFilePath = Path.Combine(OutputVideoFilePath, FileName + fileFormat);

			if (!Directory.Exists(OutputVideoFilePath))
			{
				Directory.CreateDirectory(OutputVideoFilePath);
			}
			
			var fileCount = Directory.GetFiles(OutputVideoFilePath).Length;
			if (File.Exists(_encodedFilePath))
			{
				_encodedFilePath = Path.Combine(OutputVideoFilePath, FileName + "_" + fileCount + fileFormat);
			}

			return _encodedFilePath;
		}
#endif
	}
}
