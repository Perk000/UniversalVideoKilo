using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Media;
using SilverTau.NSR.Recorders.Audio;
#endif

namespace SilverTau.NSR.Recorders
{
    public class NSREditorAudioProcessor
    {
#if UNITY_EDITOR
        private AudioListener _targetAudioListener;
        private AudioSource _targetAudioSource;
        private List<float> _audioBuffer = new List<float>();
        private readonly object _audioBufferLock = new object();
        private int _sampleFramesPerVideoFrame;
        
        public AudioTrackAttributes AudioAttributes { get; private set; }
        private List<NSRAudioReceiver> AudioReceivers { get; set; } = new List<NSRAudioReceiver>();

        private SampleBufferInputDelegate _tempSampleBufferInputDelegate;
        
        public void Initialize(int targetFrameRate, AudioListener audioListener, AudioSource audioSource, List<NSRAudioReceiver> audioReceivers = null)
        {
            int outputSampleRate = AudioSettings.outputSampleRate;
            int outputChannelCount = (int)AudioSettings.speakerMode > 2 ? 2 : (int)AudioSettings.speakerMode;
            _sampleFramesPerVideoFrame = outputChannelCount * outputSampleRate / targetFrameRate;

            _targetAudioListener = audioListener;
            _targetAudioSource = audioSource;
            AudioReceivers = audioReceivers;
            
            if (AudioReceivers != null && AudioReceivers.Count > 0)
            {
                var firstItem = AudioReceivers[0];
                firstItem.SampleBufferDelegate = OnSampleBuffer;
            }
            else
            {
                if (_targetAudioListener != null)
                {
                    _tempSampleBufferInputDelegate = _targetAudioListener.gameObject.AddComponent<SampleBufferInputDelegate>();
                }
                else
                {
                    if (_targetAudioSource != null)
                    {
                        _tempSampleBufferInputDelegate = _targetAudioSource.gameObject.AddComponent<SampleBufferInputDelegate>();
                    }
                }
            
                if (_tempSampleBufferInputDelegate != null)
                {
                    _tempSampleBufferInputDelegate.sampleBufferDelegate = OnSampleBuffer;
                }
            }
        }

        public void SetupAudioAttributes(int targetFrameRate)
        {
            var outputSampleRate = AudioSettings.outputSampleRate;
            var outputChannelCount = (int)AudioSettings.speakerMode > 2 ? (ushort)2 : (ushort)AudioSettings.speakerMode;
			
            AudioAttributes = new AudioTrackAttributes
            {
                sampleRate = new MediaRational(outputSampleRate),
                channelCount = outputChannelCount,
                language = "en"
            };
            
            _sampleFramesPerVideoFrame = AudioAttributes.channelCount * AudioAttributes.sampleRate.numerator / targetFrameRate;
        }
        
        public void CaptureAudio(MediaEncoder mediaEncoder)
        {
            float[] samplesForFrame = new float[_sampleFramesPerVideoFrame];
            lock (_audioBufferLock)
            {
                if (_audioBuffer.Count >= _sampleFramesPerVideoFrame)
                {
                    _audioBuffer.CopyTo(0, samplesForFrame, 0, _sampleFramesPerVideoFrame);
                    _audioBuffer.RemoveRange(0, _sampleFramesPerVideoFrame);
                }
                else
                {
                    int available = _audioBuffer.Count;
                    if (available > 0)
                    {
                        _audioBuffer.CopyTo(0, samplesForFrame, 0, available);
                        _audioBuffer.RemoveRange(0, available);
                    }
                }
            }
            using (var nativeBuffer = new NativeArray<float>(samplesForFrame, Allocator.Temp))
            {
                mediaEncoder?.AddSamples(nativeBuffer);
            }
        }

        public void OnSampleBuffer (float[] data) 
        {
            lock (_audioBufferLock)
            {
                if (AudioReceivers != null)
                {
                    if (AudioReceivers.Count > 0)
                    {
                        var combineAudioData = AudioHelper.CombineAudioData(AudioReceivers);
                        _audioBuffer.AddRange(combineAudioData);
                        return;
                    }
                }
			
                _audioBuffer.AddRange(data);
            }
        }
        
        public void Dispose()
        {
            if (_tempSampleBufferInputDelegate != null)
            {
                UnityEngine.Object.Destroy(_tempSampleBufferInputDelegate);
            }
            
            _tempSampleBufferInputDelegate = null;
            
            if (AudioReceivers != null)
            {
                foreach (var audioReceiver in AudioReceivers)
                {
                    UnityEngine.Object.Destroy(audioReceiver);
                }
            }

            AudioReceivers = new List<NSRAudioReceiver>();
			
            lock (_audioBufferLock)
            {
                _audioBuffer.Clear();
                _audioBuffer = new List<float>();
            }
        }
    
        private class SampleBufferInputDelegate : MonoBehaviour {
            public Action<float[]> sampleBufferDelegate;
            private void OnAudioFilterRead (float[] data, int channels) => sampleBufferDelegate?.Invoke(data);
        }
#endif
    }
}