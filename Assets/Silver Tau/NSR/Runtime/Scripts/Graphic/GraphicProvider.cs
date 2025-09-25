using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Graphic
{
    /// <summary>
    /// Image encoding format.
    /// </summary>
    [Serializable]
    public enum EncodeTo
    {
        EXR, JPG, PNG, TGA
    }
    
    /// <summary>
    /// The main path where the images will be stored.
    /// </summary>
    [Serializable]
    public enum ApplicationDataPath
    {
        PersistentDataPath, DataPath, TemporaryCachePath
    }
    
    [AddComponentMenu("Silver Tau/NSR/Graphic Provider")]
    public class GraphicProvider : MonoBehaviour
    {
        [Tooltip("The main rendering camera.")]
        public Camera mainCamera;
        
        [Tooltip("This is a parameter that contains a link to a script object that sets the general settings for the system.")]
        [SerializeField] private GraphicSettings graphicSettings;
        
        [Tooltip("Action that is called after a screenshot is successfully created.")]
        public UnityAction OnCreateImage;
        
        /// <summary>
        /// List of subsystems that perform the image rendering process.
        /// </summary>
        public List<GraphicSubsystem> GraphicSubsystems { get; } = new List<GraphicSubsystem>();

       
        #region MonoBehaviour

        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
#if UNITY_2023_1_OR_NEWER
            var imgSubsystems = FindObjectsByType<GraphicSubsystem>(FindObjectsSortMode.None);
#else
            var imgSubsystems = FindObjectsOfType<GraphicSubsystem>();
#endif

            if (imgSubsystems.Length > 0)
            {
                foreach (var subsystem in imgSubsystems)
                {
                    if(GraphicSubsystems.Contains(subsystem)) continue;
                    GraphicSubsystems.Add(subsystem);
                }
                return;
            }
            
            var renderImageSubsystem = gameObject.AddComponent<RenderGraphicSubsystem>();
            GraphicSubsystems.Add(renderImageSubsystem);
        }

        #endregion
        
        #region Render Image
        
        /// <summary>
        /// A function that creates an image using a targeted group of subsystems that provide an image.
        /// </summary>
        public void CreateImage()
        {
            foreach (var subsystem in GraphicSubsystems)
            {
                subsystem.RenderCamera = mainCamera;
                subsystem.GraphicSettings = graphicSettings;
                subsystem.CreateImage();
            }
            
            OnCreateImage?.Invoke();
        }

        #endregion
    }
}
