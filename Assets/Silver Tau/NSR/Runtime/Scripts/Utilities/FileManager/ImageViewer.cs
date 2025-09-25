using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    public class ImageViewer : MonoBehaviour
    {
        /// <summary>
        /// Path to the target image.
        /// </summary>
        public string targetPath { get; set; }
        
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Button buttonCloseImageView;
        [SerializeField] private AspectRatioFitter fullScreenRatioFitter;
        
        private void Start()
        {
            buttonCloseImageView.onClick.AddListener(CloseImageView);
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
            if (rawImage.texture)
            {
                Destroy(rawImage.texture);
            }
        }

        /// <summary>
        /// A function that performs a close action.
        /// </summary>
        private void CloseImageView()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// A function that performs the initialization action.
        /// </summary>
        public void Init()
        {
            if (rawImage == null)
            {
                gameObject.SetActive(false);
                return;
            }

            LoadImage(targetPath, texture2D =>
            {
                var width = texture2D.width;
                var height = texture2D.height;

                var ratio = width / (float)height;
            
                rawImage.texture = texture2D;
                fullScreenRatioFitter.aspectRatio = ratio;
            });
        }
        
        /// <summary>
        /// A function that performs the action of loading a picture from the input path.
        /// </summary>
        /// <param name="filePath">Input path.</param>
        /// <param name="callback">Callback.</param>
        private void LoadImage(string filePath, Action<Texture2D> callback) 
        {
            Texture2D texture = null;

            if (!File.Exists(filePath)) return;
            var fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            callback?.Invoke(texture);
        }
    }
}
