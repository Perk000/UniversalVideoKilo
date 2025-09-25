using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilverTau.Samples
{
    /// <summary>
    /// MultiSceneRecorder is responsible for handling video recording across multiple scenes.
    /// It uses a singleton pattern to ensure only one instance is active at all times.
    /// </summary>
    public class MultiSceneRecorder : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of MultiSceneRecorder.
        /// </summary>
        public static MultiSceneRecorder Instance { get; set; }

        /// <summary>
        /// A reference to the UniversalVideoRecorder that handles the actual video recording process.
        /// </summary>
        public UniversalVideoRecorder universalVideoRecorder;
        
        /// <summary>
        /// Ensures that only one instance of this class exists at a time.
        /// If another instance is found, it will be destroyed.
        /// </summary>
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start() { }

        /// <summary>
        /// Changes the active scene by its build index.
        /// </summary>
        /// <param name="index">Build index of the scene to be loaded.</param>
        public void ChangeScene(int index)
        {
            SceneManager.LoadScene(index);
        }
        
        /// <summary>
        /// Starts video recording via the UniversalVideoRecorder component.
        /// Ensures the recording camera's delegate isn't destroyed on scene load.
        /// </summary>
        public void StartVideoRecording()
        {
            if(universalVideoRecorder == null) return;
            universalVideoRecorder.StartVideoRecorder();
            
            // Starting from the plugin version 1.8.0+, this part of the code is deprecated and may not be used.
            var cameraInputDelegate = GameObject.Find("[Temp] Camera Input Delegate");
            if (cameraInputDelegate != null)
            {
                DontDestroyOnLoad(cameraInputDelegate);
            }
        }
        
        /// <summary>
        /// Stops the video recording via the UniversalVideoRecorder component.
        /// </summary>
        public void StopVideoRecording()
        {
            if(universalVideoRecorder == null) return;
            universalVideoRecorder.StopVideoRecorder();
        }
    }
}