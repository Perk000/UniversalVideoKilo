using System;
using System.Collections;
using SilverTau.NSR.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace SilverTau.NSR.Recorders.Audio
{
    [AddComponentMenu("Silver Tau/NSR/Microphone Audio Recorder")]
    public class MicrophoneAudioRecorder : MicrophoneAudioInput
    {
        public bool MicrophonePermissionsResultValue { get; set; } = false;
        public UnityAction<bool> onMicrophonePermissionsResult;

        private void OnEnable()
        {
            if(MicrophonePermissionsResultValue) return;
            StartCoroutine(MicrophonePermissions());
        }
        
        private IEnumerator MicrophonePermissions()
        {
            yield return PermissionHelper.CheckMicrophonePermissions(CheckMicrophonePermissionsResult);
        }

        private void CheckMicrophonePermissionsResult(bool value)
        {
            MicrophonePermissionsResultValue = value;
            onMicrophonePermissionsResult?.Invoke(value);
        }
    }
}