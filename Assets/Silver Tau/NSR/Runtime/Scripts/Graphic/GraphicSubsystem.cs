using System;
using UnityEngine;
using UnityEngine.Events;

namespace SilverTau.NSR.Graphic
{
    public abstract class GraphicSubsystem : MonoBehaviour
    {
        /// <summary>
        /// Identifier of the graphics subsystem.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Shared Graphic which is formed from the subsystem.
        /// </summary>
        public SharedGraphic sharedGraphic { get; set; }
        
        /// <summary>
        /// Targeted rendering camera for the subsystem.
        /// </summary>
        public Camera RenderCamera { get; set; }
        
        /// <summary>
        /// This is a parameter that contains a link to a script object that sets the general settings for the system.
        /// </summary>
        public GraphicSettings GraphicSettings { get; set; }
        
        /// <summary>
        /// An abstract function that creates an image from the targeted subsystem.
        /// </summary>
        public abstract void CreateImage();

        /// <summary>
        /// An abstract function that stores images from a targeted shared graphic.
        /// </summary>
        public abstract void SaveSharedGraphic();

        /// <summary>
        /// An action that signals that the shared graphics file has been saved and returns the path of the saved file.
        /// </summary>
        public Action<string> OnSharedGraphicSaved;
    }
}
