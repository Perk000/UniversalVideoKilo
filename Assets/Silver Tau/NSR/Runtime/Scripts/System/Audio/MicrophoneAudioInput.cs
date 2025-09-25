using System;
using System.IO;
using SilverTau.NSR.Recorders.Audio.Formats;
using UnityEngine;
using UnityEngine.Events;

namespace SilverTau.NSR.Recorders.Audio
{
    public abstract class MicrophoneAudioInput : MonoBehaviour
    {
        #region Public Variables
        
        /// <summary>
        /// The format of the output audio file.
        /// </summary>
        public AudioFormats audioFormats = AudioFormats.Automatic;
        
        /// <summary>
        /// The header size for the audio file. Be careful when changing it. The default value is 44.
        /// </summary>
        public int HeaderSize { get; set; } = 44;
        
        /// <summary>
        /// Automatic frequency detection.
        /// </summary>
        public bool setAutoFrequency = true;
        
        /// <summary>
        /// Frequency for the audio file. Be careful when changing it. The default value is 48000.
        /// </summary>
        public int frequency = 48000;
        
        /// <summary>
        /// Automatic detection of the number of channels.
        /// </summary>
        public bool setAutoChannels = true;
        
        /// <summary>
        /// The number of channels for the audio file. Be careful when changing it. The default value is 2.
        /// </summary>
        public AudioSpeakerMode channels = AudioSpeakerMode.Stereo;
        
        /// <summary>
        /// Enable/disable RMS value calculation.
        /// </summary>
        public bool computeRMS = true;
        
        /// <summary>
        /// Enable/disable Decibel value calculation.
        /// </summary>
        public bool computeDB = true;
        
        /// <summary>
        /// The size of the buffer window for calculating RMS and Decibels.
        /// </summary>
        public int bufferWindowLength = 128;
        
        /// <summary>
        /// Current RMS value.
        /// </summary>
        public float CurrentRMS => _currentRms;
        
        /// <summary>
        /// Current Decibel value.
        /// </summary>
        public float CurrentDB => _currentDB;
        
        /// <summary>
        /// The current recording settings of the AudioInputComponent.
        /// </summary>
        public AudioInputComponent CurrentAudioInputComponent => _audioInputComponent;
        
        /// <summary>
        /// Audio Source to store Microphone Input, An AudioSource Component is required by default
        /// </summary>
        public AudioSource GetAudioSource => _audioSource;
        
        /// <summary>
        /// An event that allows you to get the current sample buffer.
        /// </summary>
        public UnityAction<float[], int> sampleBufferDelegate;
        
        /// <summary>
        /// An event indicating that the recording is complete.
        /// </summary>
        public UnityAction onCompleteCapture;
        
        /// <summary>
        /// An event that indicates that an error has occurred.
        /// </summary>
        public UnityAction onErrorCapture;
        
        #endregion
        
        #region Private Variables
        
        private AudioSource _audioSource;
        private AudioInputComponent _audioInputComponent;
        
        private float _currentRms = 0;
        private float _currentDB = 0;
        
        private bool _isRecording = false;
        private string _currentDeviceName;
        
#if !NSR_MICROPHONE_DISABLE
        private int _duration = 10;
        private int _micPositionTimer = 0;
#endif

        private string _audioFilePath = "";
        private string _audioFileName = "";
        
        #endregion
        
        #region Editor
        
        [HideInInspector] public bool _mainSettingExpand = true;
        [HideInInspector] public bool _audioSettingExpand = true;
        [HideInInspector] public bool _advancedSettingExpand = true;

        #endregion
        
        /// <summary>
        /// Current microphone array.
        /// </summary>
        /// <returns></returns>
        public string[] GetMicrophoneDevices()
        {
#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            return Microphone.devices;
#else
            Debug.LogWarning("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
            return null;
#endif
        }

        private void Start() { }

        #region Prepare functions

        private bool PrepareRecorder()
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            
#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            _micPositionTimer = 0;

            if (Microphone.devices.Length == 0)
            {
                Debug.Log("The microphone array is empty!");
                return false;
            }

            StopAllCoroutines();
            
            if (setAutoFrequency)
            {
                frequency = AudioSettings.outputSampleRate;
            }
            
            if (setAutoChannels)
            {
                channels = AudioSettings.speakerMode;
            }

            _audioInputComponent = new AudioInputComponent(audioFormats, (int)channels, frequency, HeaderSize);
            sampleBufferDelegate += SampleBufferDelegate;
            
            return true;
#else
            Debug.LogWarning("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
            return false;
#endif
        }
        
        private void CheckFilePath(string filepath, string filename)
        {
            if (!string.IsNullOrEmpty(filepath))
            {
                _audioFilePath = filepath;
            }
            
            if (!string.IsNullOrEmpty(filename))
            {
                _audioFileName = filename;
            }
            
            if (string.IsNullOrEmpty(_audioFilePath))
            {
                _audioFilePath = Path.Combine(Application.persistentDataPath, "NSR - Screen Recorder", "Audio");
            
                if (!Directory.Exists(_audioFilePath))
                {
                    Directory.CreateDirectory(_audioFilePath);
                }
            }
            
            if (string.IsNullOrEmpty(_audioFileName))
            {
                _audioFileName = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            }
        }

        #endregion
        
        #region Main functions

        /// <summary>
        /// A function that starts recording an audio file.
        /// </summary>
        /// <param name="micDeviceName">Target microphone name for recording.</param>
        public void StartRecording(string micDeviceName = "")
        {
            if(!this.enabled)
            {
                return;
            }
            
            if (_isRecording)
            {
                return;
            }

            if (!PrepareRecorder())
            {
                onErrorCapture?.Invoke();
                return;
            }
#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            _currentDeviceName = Microphone.devices[0];
            
            if (!string.IsNullOrEmpty(micDeviceName))
            {
                _currentDeviceName = micDeviceName;
            }

            _audioSource.mute = _audioSource.loop = true;
            _audioSource.bypassEffects = _audioSource.bypassListenerEffects = false;
            
            _audioSource.clip = Microphone.Start(_currentDeviceName, true, _duration, frequency);
            
            if (!(Microphone.GetPosition(_currentDeviceName) > 0))
            {
                Invoke("CheckMicPosition", 0.2f);
                return;
            }
            
            _audioSource.Play();
            _audioSource.mute = false;
            _isRecording = true;
#else
            Debug.LogWarning("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
            onErrorCapture?.Invoke();
            return;
#endif
        }
        
        private void CheckMicPosition()
        {
#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            if (!(Microphone.GetPosition(_currentDeviceName) > 0))
            {
                if (_micPositionTimer >= 10)
                {
                    Dispose();
                
                    sampleBufferDelegate -= SampleBufferDelegate;
                    _audioInputComponent = null;
                
                    onErrorCapture?.Invoke();
                    return;
                }

                _micPositionTimer += 1;
                
                Invoke("CheckMicPosition", 0.2f);
                return;
            }
            
            _audioSource.Play();
            _audioSource.mute = false;
            _isRecording = true;
#else
            Debug.LogWarning("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
            return;
#endif
        }

        private void Dispose()
        {
            if (_audioSource != null)
            {
                Destroy(_audioSource);
            }
#if !NSR_MICROPHONE_DISABLE && !PLATFORM_WEBGL
            if (string.IsNullOrEmpty(_currentDeviceName))
            {
                _currentDeviceName = Microphone.devices[0];
            }

            if (Microphone.IsRecording(_currentDeviceName))
            {
                Microphone.End(_currentDeviceName);
            }

            _micPositionTimer = 0;
#else
            Debug.LogWarning("Warning: Microphone use is disabled! NSR_MICROPHONE_DISABLE was found. If you plan to use the microphone, remove this symbol from the list.");
#endif
        }
        
        /// <summary>
        /// The function of ending the audio recording.
        /// </summary>
        /// <param name="filePath">The output path of the audio file.</param>
        /// <param name="fileName">The name of the audio file.</param>
        public void StopRecording(string filePath, string fileName)
        {
            if(!this.enabled)
            {
                return;
            }

            if (!_isRecording)
            {
                return;
            }
            
            Dispose();
            
            sampleBufferDelegate -= SampleBufferDelegate;

            CheckFilePath(filePath, fileName);
            
            _audioInputComponent.Stop(_audioFilePath, _audioFileName);
            _audioInputComponent = null;
            
            StopAllCoroutines();
            onCompleteCapture?.Invoke();
            
            _isRecording = false;
        }
        
        private void OnAudioFilterRead (float[] data, int channels) => sampleBufferDelegate?.Invoke(data, channels);

        private void SampleBufferDelegate(float[] data, int channels)
        {
            AndroidJNI.AttachCurrentThread();
            _audioInputComponent.PushData(data);

            if (computeRMS)
            {
                _currentRms = AudioHelper.ComputeRMS(data, 0, ref bufferWindowLength);
            }
            
            if (computeDB)
            {
                _currentDB = AudioHelper.ComputeDB(data, 0, ref bufferWindowLength);
            }
            
            Array.Clear(data, 0, data.Length);
        }

        #endregion
    }
}