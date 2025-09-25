using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    public class AudioViewer : MonoBehaviour
    {
        /// <summary>
        /// Path to the target image.
        /// </summary>
        public string targetPath { get; set; }
        
        [SerializeField] private Button buttonStartStopAudio;
        [SerializeField] private Text textStartStopAudio;
        [SerializeField] private Button buttonCloseAudioView;

        private AudioSource _audioSource;
        
        private void Start()
        {
            buttonStartStopAudio.onClick.AddListener(StartStopAudio);
            buttonCloseAudioView.onClick.AddListener(CloseAudioView);
            
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        private void OnEnable()
        {
        }

        private void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// A function that performs the dispose action.
        /// </summary>
        private void Dispose()
        {
        }

        /// <summary>
        /// A function that performs a close action.
        /// </summary>
        private void CloseAudioView()
        {
            Destroy(gameObject);
        }
        
        private void StartStopAudio()
        {
            if(_audioSource.clip == null) { return; }

            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
                textStartStopAudio.text = "Play";
            }
            else
            {
                _audioSource.Play();
                textStartStopAudio.text = "Stop";
            }
        }

        /// <summary>
        /// A function that performs the initialization action.
        /// </summary>
        public void Init()
        {
            LoadAudio(targetPath, audioClip =>
            {
                _audioSource.clip = audioClip;
            });
        }
        
        /// <summary>
        /// A function that performs the action of loading a picture from the input path.
        /// </summary>
        /// <param name="filePath">Input path.</param>
        /// <param name="callback">Callback.</param>
        private async void LoadAudio(string filePath, Action<AudioClip> callback) 
        {
            if (!File.Exists(filePath)) return;

            var audioType = AudioType.WAV;

            if (filePath.Contains(".wav") || filePath.Contains(".WAV"))
            {
                audioType = AudioType.WAV;
            }
            else if (filePath.Contains(".mp3") || filePath.Contains(".MP3"))
            {
                audioType = AudioType.MPEG;
            }
            
            var result = await GetAudioClip(filePath, audioType, true);
            
            callback?.Invoke(result);
        }
        
        private async Task<AudioClip> GetAudioClip(string filePath, AudioType fileType, bool localFile = true)
        {
            var url = localFile ? string.Format("file://{0}", filePath) : filePath; 
            
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, fileType))
            {
                var result = www.SendWebRequest();
 
                while (!result.isDone) { await Task.Delay(100); }
 
                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                    return null;
                }
                else
                {
                    return DownloadHandlerAudioClip.GetContent(www);
                }
            }
        }
    }
}
