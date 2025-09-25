using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class MenuNSRModeSelector : MonoBehaviour
    {
        [Header("UI")]
        [Space(10)]
        [SerializeField] private Button btnMode1;
        [SerializeField] private Button btnMode2;
        [SerializeField] private Button btnBack;
        [SerializeField] private GameObject body;
        [SerializeField] private GameObject menuMode1;
        [SerializeField] private GameObject menuMode2;
        
        [SerializeField] private GameObject graphicPreview;
        
        #region MonoBehaviour

        private void Start()
        {
            if(btnMode1) btnMode1.onClick.AddListener(() => SelectMode(0));
            if(btnMode2) btnMode2.onClick.AddListener(() => SelectMode(1));
            if(btnBack) btnBack.onClick.AddListener(() => SelectMode(-1));
        }
        
        #endregion

        #region Mode Selector

        private void SelectMode(int mode)
        {
            switch (mode)
            {
                case -1:
                    body.SetActive(true);
                    btnBack.gameObject.SetActive(false);
                    menuMode1.SetActive(false);
                    menuMode2.SetActive(false);
                    if(graphicPreview) graphicPreview.SetActive(false);
                    break;
                case 0:
                    body.SetActive(false);
                    btnBack.gameObject.SetActive(true);
                    menuMode1.SetActive(true);
                    menuMode2.SetActive(false);
                    if(graphicPreview) graphicPreview.SetActive(true);
                    break;
                case 1:
                    body.SetActive(false);
                    btnBack.gameObject.SetActive(true);
                    menuMode1.SetActive(false);
                    menuMode2.SetActive(true);
                    if(graphicPreview) graphicPreview.SetActive(true);
                    break;
                default:
                    body.SetActive(true);
                    btnBack.gameObject.SetActive(false);
                    menuMode1.SetActive(false);
                    menuMode2.SetActive(false);
                    if(graphicPreview) graphicPreview.SetActive(false);
                    break;
            }
        }
        
        #endregion
    }
}
