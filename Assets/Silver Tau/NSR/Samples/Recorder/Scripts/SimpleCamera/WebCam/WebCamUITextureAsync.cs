using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace SilverTau.NSR.Samples
{
    public class WebCamUITextureAsync : MonoBehaviour
    {
        [Header("UI Components")]
        public RawImage phoneCamera;
        public AspectRatioFitter aspectRatioFitter;

        [Header("Camera Settings")]
        [Tooltip("Name of the camera device or its index (if the string is a number)")]
        public string requestedDeviceName = "";
        [Tooltip("Camera width (e.g., 320)")]
        public int requestedWidth = 320;
        [Tooltip("Camera height (e.g., 240)")]
        public int requestedHeight = 240;
        [Tooltip("Camera FPS (e.g., 30)")]
        public int requestedFPS = 30;
        [Tooltip("Use the front-facing camera?")]
        public bool requestedIsFrontFacing = false;

        [Header("Additional Settings")]
        [Tooltip("Adjust the image according to screen orientation")]
        public bool adjustPixelsDirection = false;
        [Tooltip("Flip vertically")]
        public bool flipVertical = false;
        [Tooltip("Flip horizontally")]
        public bool flipHorizontal = false;
        [Tooltip("Enable debug logging")]
        public bool debug = false;

        [Header("Shader for Transformation (optional)")]
        [Tooltip("Shader for rotation/flip. A new material will be created from this shader on enable and destroyed on disable. Default shader -> Silver Tau/NSR - Screen Recorder/Web Camera/FlipRotateShader")]
        public Shader transformShader;

        private Material _transformMaterial;

        // Internal variables
        private WebCamTexture _webCamTexture;
        private RenderTexture _renderTexture;
        private Texture2D _outputTexture;
        private bool _isReadbackRequested = false;
        private bool _hasInitDone = false;
        private ScreenOrientation _screenOrientation;
        private int _screenWidth;
        private int _screenHeight;

        private void OnEnable()
        {
            if (transformShader != null)
            {
                _transformMaterial = new Material(transformShader);
            }
            Initialize();
        }

        private void OnDisable()
        {
            Dispose();
            if (_transformMaterial != null)
            {
                Destroy(_transformMaterial);
                _transformMaterial = null;
            }
        }

        /// <summary>
        /// Initializes the camera using a coroutine.
        /// </summary>
        private void Initialize()
        {
            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine()
        {
            if (!string.IsNullOrEmpty(requestedDeviceName))
            {
                int requestedDeviceIndex;
                if (int.TryParse(requestedDeviceName, out requestedDeviceIndex))
                {
                    if (requestedDeviceIndex >= 0 && requestedDeviceIndex < WebCamTexture.devices.Length)
                    {
                        _webCamTexture = new WebCamTexture(WebCamTexture.devices[requestedDeviceIndex].name, requestedWidth, requestedHeight, requestedFPS);
                    }
                }
                else
                {
                    foreach (var device in WebCamTexture.devices)
                    {
                        if (device.name == requestedDeviceName)
                        {
                            _webCamTexture = new WebCamTexture(device.name, requestedWidth, requestedHeight, requestedFPS);
                            break;
                        }
                    }
                }
            }

            if (_webCamTexture == null)
            {
                foreach (var device in WebCamTexture.devices)
                {
                    if (device.isFrontFacing == requestedIsFrontFacing)
                    {
                        _webCamTexture = new WebCamTexture(device.name, requestedWidth, requestedHeight, requestedFPS);
                        break;
                    }
                }
            }

            if (_webCamTexture == null && WebCamTexture.devices.Length > 0)
            {
                _webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, requestedWidth, requestedHeight, requestedFPS);
            }

            if (_webCamTexture == null)
            {
                if (debug)
                    Debug.LogError("Camera not found!");
                yield break;
            }

            _webCamTexture.Play();

            while (_webCamTexture.width < 16 || _webCamTexture.height < 16)
            {
                yield return null;
            }

            _renderTexture = new RenderTexture(requestedWidth, requestedHeight, 24, RenderTextureFormat.ARGB32);
            _renderTexture.Create();

            _outputTexture = new Texture2D(requestedWidth, requestedHeight, TextureFormat.RGBA32, false);
            phoneCamera.texture = _outputTexture;

            if (aspectRatioFitter != null)
            {
                float ratio = (float)requestedWidth / requestedHeight;
                aspectRatioFitter.aspectRatio = ratio;
            }

            _screenOrientation = Screen.orientation;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            _hasInitDone = true;
        }

        private void Update()
        {
            if (!_hasInitDone)
                return;

            if (adjustPixelsDirection)
            {
                if (_screenOrientation != Screen.orientation || _screenWidth != Screen.width || _screenHeight != Screen.height)
                {
                    if (debug)
                        Debug.Log("Screen orientation/size changed. Reinitializing camera.");
                    Dispose();
                    Initialize();
                    return;
                }
            }

            if (_webCamTexture.didUpdateThisFrame && !_isReadbackRequested)
            {
                if (_transformMaterial != null)
                {
                    _transformMaterial.SetInt("_FlipVertical", flipVertical ? 1 : 0);
                    _transformMaterial.SetInt("_FlipHorizontal", flipHorizontal ? 1 : 0);
                }

                if (_transformMaterial != null)
                {
                    Graphics.Blit(_webCamTexture, _renderTexture, _transformMaterial);
                }
                else
                {
                    Graphics.Blit(_webCamTexture, _renderTexture);
                }

                _isReadbackRequested = true;
                AsyncGPUReadback.Request(_renderTexture, 0, OnCompleteReadback);
            }
        }

        /// <summary>
        /// Callback after the asynchronous GPU readback is complete.
        /// </summary>
        /// <param name="request">The GPU readback request</param>
        void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            _isReadbackRequested = false;
            if (request.hasError)
            {
                if (debug)
                    Debug.LogError("Error during GPU readback.");
                return;
            }
            
            _outputTexture.LoadRawTextureData(request.GetData<byte>());
            _outputTexture.Apply();
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        void Dispose()
        {
            _hasInitDone = false;
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
                Destroy(_webCamTexture);
                _webCamTexture = null;
            }
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
                _renderTexture = null;
            }
            if (_outputTexture != null)
            {
                Destroy(_outputTexture);
                _outputTexture = null;
            }
        }

        void OnDestroy()
        {
            Dispose();
            if (_transformMaterial != null)
            {
                Destroy(_transformMaterial);
                _transformMaterial = null;
            }
        }

        /// <summary>
        /// Switches the camera (front/back).
        /// </summary>
        public void OnChangeCamera()
        {
            requestedIsFrontFacing = !requestedIsFrontFacing;
            Dispose();
            Initialize();
        }

        /// <summary>
        /// Toggles vertical flipping.
        /// </summary>
        public void ToggleVerticalFlip()
        {
            flipVertical = !flipVertical;
        }

        /// <summary>
        /// Toggles horizontal flipping.
        /// </summary>
        public void ToggleHorizontalFlip()
        {
            flipHorizontal = !flipHorizontal;
        }
    }

}
