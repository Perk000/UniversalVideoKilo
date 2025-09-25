using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class DemoDynamicCameraChanges : MonoBehaviour
    {
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        [Header("UI")]
        [Space(10)]
        [SerializeField] private Button btnStartScreenRecorder;
        [SerializeField] private Button btnStopScreenRecorder;
        [SerializeField] private Button btnNextCamera;
        [SerializeField] private Button btnPreviousCamera;
        [SerializeField] private GameObject recordMark;
        
        private bool _isRecording;
        private int _cameraIndex;

        #region MonoBehaviour

        private void Start()
        {
            if(btnStartScreenRecorder) btnStartScreenRecorder.onClick.AddListener(StartScreenRecorder);
            if(btnStopScreenRecorder) btnStopScreenRecorder.onClick.AddListener(StopScreenRecorder);
            if(btnNextCamera) btnNextCamera.onClick.AddListener(NextCamera);
            if(btnPreviousCamera) btnPreviousCamera.onClick.AddListener(PreviousCamera);
            
            _isRecording = false;
            if(recordMark) recordMark.SetActive(false);
            _cameraIndex = 0;
        }
        
        private void OnDisable()
        {
            _isRecording = false;
            if(recordMark) recordMark.SetActive(false);
            _cameraIndex = 0;
        }

        #endregion
        
        #region Buttons

        private void StartScreenRecorder()
        {
            if (_isRecording) return;
            
            universalVideoRecorder.StartVideoRecorder();
            if(recordMark) recordMark.SetActive(true);
            _isRecording = true;
        }

        private void StopScreenRecorder()
        {
            if (!_isRecording) return;
            
            universalVideoRecorder.StopVideoRecorder();
            if(recordMark) recordMark.SetActive(false);
            _isRecording = false;
        }

        private void NextCamera()
        {
            if (universalVideoRecorder.cameras.Count == 0) return;
            
            if(_cameraIndex == universalVideoRecorder.cameras.Count - 1) return;

            _cameraIndex += 1;
            ActiveCamera(_cameraIndex);
        }

        private void PreviousCamera()
        {
            if (universalVideoRecorder.cameras.Count == 0) return;
            
            if(_cameraIndex <= 0) return;

            _cameraIndex -= 1;
            ActiveCamera(_cameraIndex);
        }

        private void ActiveCamera(int index)
        {
            for (int i = 0; i < universalVideoRecorder.cameras.Count; i++)
            {
                var targetCamera = universalVideoRecorder.cameras[i];
                if(targetCamera == null) continue;
                
                targetCamera.enabled = i == index;
                targetCamera.gameObject.SetActive(i == index);
            }
        }
        
        #endregion
    }
}
