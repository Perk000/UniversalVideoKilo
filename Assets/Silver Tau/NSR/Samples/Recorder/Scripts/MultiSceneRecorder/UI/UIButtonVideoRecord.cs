using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.Samples
{
    public enum UIButtonRecordTarget
    {
        Run,
        Stop
    }
    
    [RequireComponent(typeof(Button))]
    public class UIButtonVideoRecord : MonoBehaviour
    {
        [SerializeField] private UIButtonRecordTarget recordTarget = UIButtonRecordTarget.Run;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                switch (recordTarget)
                {
                    case UIButtonRecordTarget.Run:
                        Run();
                        break;
                    case UIButtonRecordTarget.Stop:
                        Stop();
                        break;
                    default:
                        return;
                }
            });
        }

        private void Start()
        {
        
        }

        private void Run()
        {
            MultiSceneRecorder.Instance.StartVideoRecording();
        }

        private void Stop()
        {
            MultiSceneRecorder.Instance.StopVideoRecording();
        }
    }
}