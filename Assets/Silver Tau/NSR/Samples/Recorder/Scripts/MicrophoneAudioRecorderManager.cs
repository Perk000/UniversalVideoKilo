using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SilverTau.NSR.Recorders.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class MicrophoneAudioRecorderManager : MonoBehaviour
    {
        [Header("UI")]
        [Space(10)]
        [SerializeField] private MicrophoneAudioRecorder microphoneAudioRecorder;
        [SerializeField] private Button buttonStartStop;
        [SerializeField] private Button buttonRefreshMicrophoneDevices;
        [SerializeField] private Text textStartStop;
        [SerializeField] private Text textTime;
        [SerializeField] private Image imageDBVisualizer;
        [SerializeField] private Dropdown dropdownMicrophoneDevices;

        private bool _isRecording;
        private string[] _microphoneDevices = Array.Empty<string>();
        private string _currentMicrophoneDevice;
        private int _currentMicrophoneDeviceSelection;

        private string _audioFilePath;
        private string _audioFileName;
        
        #region MonoBehaviour

        private void OnEnable()
        {
            microphoneAudioRecorder.onMicrophonePermissionsResult += OnMicrophonePermissionsResult;
        }

        private void OnDisable()
        {
            microphoneAudioRecorder.onMicrophonePermissionsResult -= OnMicrophonePermissionsResult;
        }


        private void Start()
        {
            if (imageDBVisualizer) imageDBVisualizer.fillAmount = 0;
            if (buttonStartStop) buttonStartStop.onClick.AddListener(StartStopAudioRecorder);
            if (buttonRefreshMicrophoneDevices) buttonRefreshMicrophoneDevices.onClick.AddListener(RefreshMicrophoneDevices);
            if (textStartStop) textStartStop.text = "Start";

            if (microphoneAudioRecorder.MicrophonePermissionsResultValue)
            {
                RefreshMicrophoneDevices();
            }
            
            if (dropdownMicrophoneDevices) dropdownMicrophoneDevices.onValueChanged.AddListener(MicrophoneDevicesOnValueChanged);
            
            _audioFilePath = Path.Combine(Application.persistentDataPath, "NSR - Screen Recorder", "Audio");
            
            if (!Directory.Exists(_audioFilePath))
            {
                Directory.CreateDirectory(_audioFilePath);
            }
        }

        private void MicrophoneDevicesOnValueChanged(int value)
        {
            _currentMicrophoneDevice = _microphoneDevices[value];
            _currentMicrophoneDeviceSelection = value;
        }

        private void OnMicrophonePermissionsResult(bool value)
        {
            if(!value) return;
            RefreshMicrophoneDevices();
        }

        private void RefreshMicrophoneDevices()
        {
            var microphoneDevices = microphoneAudioRecorder.GetMicrophoneDevices();
            if(microphoneDevices.Length == _microphoneDevices.Length) { return; }

            _microphoneDevices = microphoneDevices;
            dropdownMicrophoneDevices.ClearOptions();

            if (_microphoneDevices.Length <= _currentMicrophoneDeviceSelection)
            {
                _currentMicrophoneDevice = _microphoneDevices.Length > 0 ? _microphoneDevices[0] : null;
            }
            else
            {
                _currentMicrophoneDevice = _microphoneDevices[_currentMicrophoneDeviceSelection];
            }
            
            if (microphoneDevices.Length == 0)
            {
                return;
            }

            dropdownMicrophoneDevices.AddOptions(microphoneDevices.ToList());
        }

        #endregion

        #region Main Logic

        private void StartStopAudioRecorder()
        {
            if (!_isRecording)
            {
                if (!microphoneAudioRecorder.GetMicrophoneDevices().Contains(_currentMicrophoneDevice))
                {
                    RefreshMicrophoneDevices();
                }
                
                microphoneAudioRecorder.StartRecording(_currentMicrophoneDevice);

                if (textStartStop) textStartStop.text = "Stop";
                dropdownMicrophoneDevices.interactable = false;
                _isRecording = true;
                return;
            }

            _audioFileName = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            
            microphoneAudioRecorder.StopRecording(_audioFilePath, _audioFileName);
            if (textStartStop) textStartStop.text = "Start";
            dropdownMicrophoneDevices.interactable = true;
            _isRecording = false;
        }

        private void Update()
        {
            if(!_isRecording) { return; }
            if(imageDBVisualizer) imageDBVisualizer.fillAmount = AudioHelper.DecibelToLinear(microphoneAudioRecorder.CurrentDB) * 2;
        }

        #endregion
    }
}
