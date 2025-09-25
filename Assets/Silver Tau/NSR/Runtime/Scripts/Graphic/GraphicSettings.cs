using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    [CreateAssetMenu(fileName = "Graphic Settings", menuName = "Silver Tau/NSR/Graphic/Graphic Settings", order = 1)]
    public class GraphicSettings : ScriptableObject
    {
        [Tooltip("Layers that will be displayed when you take a screenshot.")]
        public LayerMask screenshotLayerMasks;
        [Tooltip("The part of the original path that will be used to save the image.")]
        public string filePath = "app/screenshots";
        [Tooltip("The main path where the images will be stored.")]
        public ApplicationDataPath applicationDataPath = ApplicationDataPath.PersistentDataPath;
        [Tooltip("Image encoding format.")]
        public EncodeTo encodeTo = EncodeTo.PNG;
        [Tooltip("HDR (high dynamic range) is used for a wide range of colors. Use this option when using HDR colors for materials or post-processing (e.g., Bloom effect).")]
        public bool HDR = false;
        [Tooltip("Graphic image quality. Only for .jpg format.")]
        [Range(0, 100)] public int imageQuality = 80;
        [Tooltip("A parameter that allows you to use your own time format for a screenshot.")]
        public bool addDateTimeToGraphicName;
        [Tooltip("Custom time format for screenshots.")]
        public string dateTimeFormat = "MMddyyyyHHmmss000";
        [Tooltip("A parameter that gives a custom name to the image.")]
        public string graphicName = "screenshot_demo";
        [Tooltip("A parameter that allows you to automatically clear the memory after the subsystem creates an image.")]
        public bool autoClearMemory = true;
        [Tooltip("A parameter that allows you to automatically save images after they are created by the subsystem.")]
        public bool autoSaveSharedGraphic = true;
        [Tooltip("A parameter that allows you to automatically delete images after the subsystem creates an image.")]
        public bool deleteImageAfterSave = false;
        [Tooltip("Sets the divider for the output image from the XR camera. 1 is the default value, which represents the original image size.")]
        public float imageSize = 1;
        [Tooltip("Checks the maximum output size of the image by width. If the image is larger than this check, the output image divider \"imageSize\" will be used.")]
        public float changeImageSizeIfWidthMore = 1024;
        [Tooltip("Parameter to check the maximum output image size in height. If the image is larger than this check, the output image divider \"imageSize\" will be used.")]
        public float changeImageSizeIfHeightMore = 1024;

    }
}
