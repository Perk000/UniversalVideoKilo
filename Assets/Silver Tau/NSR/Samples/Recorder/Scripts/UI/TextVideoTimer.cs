using System;
using System.Collections;
using System.Collections.Generic;
using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    [RequireComponent(typeof(Text))]
    public class TextVideoTimer : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        
        private Text _textTimer;
        
        private float _seconds = 0.0f;
        private float _minutes = 0.0f;
        private float _hours = 0.0f;

        private bool _isRecording;

        private void OnEnable()
        {
            if (_textTimer == null) _textTimer = GetComponent<Text>();
            
            universalVideoRecorder.onStartCapture += OnStartCapture;
            universalVideoRecorder.onPauseCapture += OnPauseCapture;
            universalVideoRecorder.onResumeCapture += OnResumeCapture;
            universalVideoRecorder.onStopCapture += OnStopCapture;
        }

        private void OnStopCapture()
        {
            _isRecording = false;
            Dispose();
        }

        private void OnResumeCapture()
        {
            _isRecording = true;
        }

        private void OnPauseCapture()
        {
            _isRecording = false;
        }

        private void OnStartCapture()
        {
            Run();
            _isRecording = true;
        }

        private void ResetObject()
        {
            _seconds = 0.0f;
            _minutes = 0.0f;
            _hours = 0.0f;
            
            if (_textTimer) _textTimer.text = $"{_hours:00}:{_minutes:00}:{_seconds:00}";
        }

        public void Dispose()
        {
            ResetObject();
            if (_textTimer) _textTimer.enabled = false;
        }

        private void Run()
        {
            ResetObject();
            if (_textTimer) _textTimer.enabled = true;
        }

        private void OnDisable()
        {
            Dispose();
            
            universalVideoRecorder.onStartCapture -= OnStartCapture;
            universalVideoRecorder.onPauseCapture -= OnPauseCapture;
            universalVideoRecorder.onResumeCapture -= OnResumeCapture;
            universalVideoRecorder.onStopCapture -= OnStopCapture;
        }

        private void Update()
        {
            if (!_isRecording) return;
            
            _seconds += Time.deltaTime;
            
            if (_seconds >= 59)
            {
                if (_minutes >= 59)
                {
                    _hours += 1.0f;
                    _minutes = 0.0f;
                }

                _minutes += 1.0f;
                _seconds = 0.0f;
            }
            
            if (_textTimer)
            {
                _textTimer.text = $"{_hours:00}:{_minutes:00}:{_seconds:00}";
            }
        }
    }
}
