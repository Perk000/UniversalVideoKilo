using UnityEngine;

namespace SilverTau.Samples
{
    /// <summary>
    /// Handles basic device settings at the start of the scene,
    /// such as preventing the screen from sleeping and setting the target frame rate.
    /// </summary>
    public class DeviceSettings : MonoBehaviour
    {
        [Header("Screen:")]
        [Tooltip("If enabled, prevents the screen from going to sleep while the app is running.")]
        [SerializeField] private bool screenNeverSleep = true;
        
        [Header("FrameRate:")]
        [Tooltip("Enable to set a target frame rate for the application.")]
        [SerializeField] private bool useTargetFrameRate = true;

        [Tooltip("The desired target frame rate for the application.")]
        [SerializeField] private int targetFrameRate = 60;
        
        private void Start()
        {
            if (screenNeverSleep)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (useTargetFrameRate)
            {
                Application.targetFrameRate = targetFrameRate;
            }
        }
    }
}