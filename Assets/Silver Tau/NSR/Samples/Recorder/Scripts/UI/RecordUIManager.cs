using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class RecordUIManager : MonoBehaviour
    {
        
        [Tooltip("Targeted UI canvas.")]
        [SerializeField]private Canvas canvas;
        
        [Tooltip("Targeted UI toggle.")]
        [SerializeField] private Toggle tgRecordUI;
        
        private void Start()
        {
            tgRecordUI.onValueChanged.AddListener(ChangeRecordUIStatus);
        }

        /// <summary>
        /// A function that changes the Render mode of the target UI canvas.
        /// </summary>
        /// <param name="value">Toggle value.</param>
        private void ChangeRecordUIStatus(bool value)
        {
            var uvr = UniversalVideoRecorder.Instance;
            
            if (!value)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                if (uvr) uvr.ConfigInputUILayer = false;
                return;
            }
            
            canvas.worldCamera = uvr != null ? uvr.mainCamera : Camera.main;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            if (uvr) uvr.ConfigInputUILayer = true;
        }
    }
}
