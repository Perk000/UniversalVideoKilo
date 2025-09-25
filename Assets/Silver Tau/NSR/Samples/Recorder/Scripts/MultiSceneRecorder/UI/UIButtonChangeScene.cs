using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.Samples
{
    [RequireComponent(typeof(Button))]
    public class UIButtonChangeScene : MonoBehaviour
    {
        [SerializeField] private int sceneIndex = 0;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ChangeScene);
        }

        private void Start()
        {
        
        }

        private void ChangeScene()
        {
            MultiSceneRecorder.Instance.ChangeScene(sceneIndex);
        }
    }
}