
using System;
using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    public class RenderGraphicSubsystem : GraphicSubsystem
    {
        private void Awake()
        {
            Id = "RenderGraphicSubsystem";
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        #region Render
        
        /// <summary>
        /// A function that creates a SharedGraphic from the target RenderTexture.
        /// </summary>
        private void CreateSharedGraphicFromRenderTexture()
        {
            if (RenderCamera == null)
            {
                sharedGraphic = null;
                return;
            }

            var targetRenderTexture = RenderCamera.targetTexture;
            
            var width = !targetRenderTexture ? Screen.width : targetRenderTexture.width;
            var height = !targetRenderTexture ? Screen.height : targetRenderTexture.height;
            
            var renderTexture = !targetRenderTexture ? new RenderTexture(width, height, 24, GraphicSettings.HDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32) : targetRenderTexture;
            var tempTexture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            
            var defaultCullingMask = RenderCamera.cullingMask;
            RenderCamera.cullingMask = 0;
            RenderCamera.cullingMask = GraphicSettings.screenshotLayerMasks;
            
            if (!targetRenderTexture)
            {
                RenderCamera.targetTexture = renderTexture;
                RenderCamera.Render();
            }
            
            RenderTexture.active = renderTexture;
            tempTexture2D.ReadPixels(RenderCamera.pixelRect, 0, 0);
            tempTexture2D.Apply(false);
            
            
            if (!targetRenderTexture)
            {
                RenderCamera.targetTexture = null;
                UnityEngine.Object.Destroy(renderTexture);
                renderTexture = null;
            }
            
            RenderTexture.active = null;
 
            RenderCamera.cullingMask = defaultCullingMask;
            
            if (GraphicSettings.autoClearMemory)
            {
                if (sharedGraphic != null)
                {
                    if (sharedGraphic.texture)
                    {
                        DestroyImmediate(sharedGraphic.texture, true);
                    }
                }
            }

            CreateSharedGraphicName(tempTexture2D);
        }
        
        private bool CheckCameraRenderTexture()
        {
            return RenderCamera.targetTexture;
        }

        /// <summary>
        /// A function that creates a SharedGraphic.
        /// </summary>
        /// <param name="sharedGraphic">Target SharedGraphic.</param>
        private void CreateSharedGraphicName(Texture2D texture2D = null)
        {
            sharedGraphic = new SharedGraphic();
            var resultGraphicName = GraphicSettings.graphicName;
            resultGraphicName += GraphicSettings.addDateTimeToGraphicName
                ? string.IsNullOrEmpty(GraphicSettings.dateTimeFormat) 
                    ? "_" + DateTime.Now.ToString() 
                    : "_" + DateTime.Now.ToString(GraphicSettings.dateTimeFormat)
                : "";

            sharedGraphic.name = resultGraphicName;
            sharedGraphic.id = System.Guid.NewGuid().ToString();

            if (texture2D != null)
            {
                sharedGraphic.texture = texture2D;
            }
        }
        
        #endregion

        /// <summary>
        /// A function that creates an image from the targeted subsystem.
        /// </summary>
        public override void CreateImage()
        {
            CreateSharedGraphicFromRenderTexture();
            SaveSharedGraphic();
        }

        /// <summary>
        /// A function that saves images from the targeted shared graphic.
        /// </summary>
        public override void SaveSharedGraphic()
        {
            if(GraphicSettings == null) return;
            if(!GraphicSettings.autoSaveSharedGraphic) return;
            
            sharedGraphic?.SaveToFile(GraphicSettings.applicationDataPath, GraphicSettings.filePath, GraphicSettings.encodeTo, GraphicSettings.imageQuality, true, (result, path) =>
            {
                OnSharedGraphicSaved?.Invoke(path);
                
                if (GraphicSettings.deleteImageAfterSave && result)
                {
                    DestroyImmediate(sharedGraphic.texture, true);
                }
            });
        }
    }
}
