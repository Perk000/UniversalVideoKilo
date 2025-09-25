using SilverTau.NSR.Recorders.Video;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Watermark
{
    /// <summary>
    /// NSR - Watermark is responsible for overlaying a watermark onto video frames rendered by the UniversalVideoRecorder.
    /// It initializes a WatermarkRenderer with configurable shader, texture, opacity, position, and size parameters.
    /// </summary>
    public class NSRWatermark : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("Reference to the Universal Video Recorder component used for recording video frames.")]
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        
        [Header("Settings")]
        [Tooltip("Shader used for watermark rendering.")]
        [SerializeField] private Shader shader;
        [Tooltip("Texture of the watermark to overlay on video frames.")]
        [SerializeField] private Texture watermarkTexture;
        [Tooltip("Opacity of the watermark (0 = transparent, 1 = opaque).")]
        [SerializeField] [Range(0.0f, 1.0f)] private float opacity = 0.5f;
        [Tooltip("Normalized position of the watermark on the video frame.")]
        [SerializeField] private Vector2 position = new Vector2(0.8f, 0.1f);
        [Tooltip("Normalized size of the watermark relative to the video frame.")]
        [SerializeField] private Vector2 size = new Vector2(0.2f, 0.2f);
        [Tooltip("Enable automatic adjustment of the watermark size based on its texture's aspect ratio.")]
        [SerializeField] private bool adjustSizeToAspect = true;
        [Tooltip("Enables a safer watermark blending method using a temporary RenderTexture. Recommended for Metal (macOS) and URP to prevent flickering or rendering artifacts.")]
        [SerializeField] private bool useSafeGPUBlit = true;
        
        private WatermarkRenderer _watermarkRenderer;
        private Vector2Int _frameSize;
        private bool _firstFramePassed;
        
        private void Start()
        {
        
        }
        
        /// <summary>
        /// Called when the object becomes enabled and active.
        /// Initializes the WatermarkRenderer and subscribes to the UniversalVideoRecorder's frame render event.
        /// </summary>
        private void OnEnable()
        {
            if(universalVideoRecorder == null) return;

            _firstFramePassed = false;
            
            _watermarkRenderer = new WatermarkRenderer(shader)
            {
                Opacity = opacity,
                Position = position,
                Size = size,
                UseSafeGPUBlit = useSafeGPUBlit
            };
            
            universalVideoRecorder.onFrameRender += OnFrameRender;
        }

        /// <summary>
        /// Called when the object becomes disabled or inactive.
        /// Unsubscribes from the frame render event and disposes of the WatermarkRenderer.
        /// </summary>
        private void OnDisable()
        {
            if(universalVideoRecorder == null) return;

            universalVideoRecorder.onFrameRender -= OnFrameRender;
            
            _watermarkRenderer?.Dispose();
            _watermarkRenderer = null;
            _firstFramePassed = false;
        }
        
        /// <summary>
        /// Callback method invoked on each video frame render.
        /// Applies the watermark overlay onto the rendered frame.
        /// </summary>
        /// <param name="frame">The current video frame as a Texture.</param>
        private void OnFrameRender(Texture frame)
        {
            if(watermarkTexture == null) return;
            _frameSize = new Vector2Int(frame.width, frame.height);

            if (!_firstFramePassed && adjustSizeToAspect)
            {
                _watermarkRenderer.Size = CalculateAspectAdjustedSize();
                _firstFramePassed = true;
            }
            
            _watermarkRenderer.CombineIntoSingleTexture(frame, watermarkTexture, _frameSize);
        }

        #region Additional methods
        
        /// <summary>
        /// Calculates the watermark size adjusted by the watermark texture's aspect ratio,
        /// taking into account the current frame size.
        /// The size is determined based on the current 'size' field as a maximum bounding box.
        /// </summary>
        /// <returns>Adjusted normalized size as a Vector2 preserving the watermark texture's aspect ratio.</returns>
        private Vector2 CalculateAspectAdjustedSize()
        {
            if (watermarkTexture == null || _frameSize.x == 0 || _frameSize.y == 0)
            {
                return size;
            }
    
            float maxPixelWidth = size.x * _frameSize.x;
            float maxPixelHeight = size.y * _frameSize.y;

            float textureAspect = (float)watermarkTexture.width / watermarkTexture.height;
    
            float adjustedPixelWidth, adjustedPixelHeight;
    
            float potentialHeight = maxPixelWidth / textureAspect;
            if (potentialHeight <= maxPixelHeight)
            {
                adjustedPixelWidth = maxPixelWidth;
                adjustedPixelHeight = potentialHeight;
            }
            else
            {
                adjustedPixelHeight = maxPixelHeight;
                adjustedPixelWidth = maxPixelHeight * textureAspect;
            }
    
            float normalizedWidth = adjustedPixelWidth / _frameSize.x;
            float normalizedHeight = adjustedPixelHeight / _frameSize.y;
    
            return new Vector2(normalizedWidth, normalizedHeight);
        }
        
        /// <summary>
        /// Changes the watermark's position.
        /// </summary>
        /// <param name="newPosition">New normalized position for the watermark.</param>
        public void ChangePosition(Vector2 newPosition)
        {
            position = newPosition;
            
            if(_watermarkRenderer == null) return;
            _watermarkRenderer.Position = position;
        }
        
        /// <summary>
        /// Changes the watermark's size.
        /// </summary>
        /// <param name="newSize">New normalized size for the watermark.</param>
        public void ChangeSize(Vector2 newSize)
        {
            size = newSize;
            
            if(_watermarkRenderer == null) return;
            _watermarkRenderer.Size = size;
        }
        
        /// <summary>
        /// Changes the watermark's opacity.
        /// </summary>
        /// <param name="newOpacity">New opacity value (0 = transparent, 1 = opaque).</param>
        public void ChangeOpacity(float newOpacity)
        {
            opacity = newOpacity;
            
            if(_watermarkRenderer == null) return;
            _watermarkRenderer.Opacity = opacity;
        }
        
        /// <summary>
        /// Updates the watermark texture.
        /// </summary>
        /// <param name="newWatermarkTexture">New texture to be used as the watermark.</param>
        public void ChangeWatermarkTexture(Texture newWatermarkTexture)
        {
            watermarkTexture = newWatermarkTexture;
        }
        
        /// <summary>
        /// Updates the shader used for watermark rendering.
        /// </summary>
        /// <param name="newShader">New shader to be applied for rendering the watermark.</param>
        public void ChangeShader(Shader newShader)
        {
            shader = newShader;
        }
        
        public void UpdateSettingsRealtime()
        {
            if(_watermarkRenderer == null) return;
            
            _watermarkRenderer.Shader = shader;
            _watermarkRenderer.Opacity = opacity;
            _watermarkRenderer.Position = position;
            _watermarkRenderer.Size = size;
            _watermarkRenderer.UseSafeGPUBlit = useSafeGPUBlit;
        }
        
        #endregion
    }
}