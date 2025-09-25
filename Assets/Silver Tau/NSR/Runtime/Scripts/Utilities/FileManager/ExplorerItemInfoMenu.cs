using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    public class ExplorerItemInfoMenu : MonoBehaviour
    {
        /// <summary>
        /// Target element Explorer Item
        /// </summary>
        public ExplorerItem ExplorerItem { get; set; }
        
        [SerializeField] private Image icon;
        [SerializeField] private Text title;
        [SerializeField] private Text info;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonOpen;
        [SerializeField] private Button buttonShare;
        [SerializeField] private Button buttonSave;
        [SerializeField] private Button buttonDelete;
        
        private void Start()
        {
            buttonBack.onClick.AddListener(Back);
            buttonOpen.onClick.AddListener(Open);
            buttonShare.onClick.AddListener(Share);
            buttonSave.onClick.AddListener(Save);
            buttonDelete.onClick.AddListener(Delete);
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            Dispose();
        }

        /// <summary>
        /// A function that performs the dispose action.
        /// </summary>
        private void Dispose()
        {
            ExplorerItem = null;
            icon.sprite = null;
            title.text = string.Empty;
            info.text = string.Empty;
        }

        /// <summary>
        /// The function that performs the action of initializing the information menu.
        /// </summary>
        private void Init()
        {
            if(ExplorerItem == null) return;
            if(ExplorerItem.Info == null) return;
            
            icon.sprite = ExplorerItem.Info.icon;
            title.text = ExplorerItem.Info.name;
            info.text = string.Empty;

            if (string.IsNullOrEmpty(ExplorerItem.Info.path)) return;
            if (!File.Exists(ExplorerItem.Info.path)) return;
            var fileInfo = new FileInfo(ExplorerItem.Info.path);
            var directoryInfo = new DirectoryInfo(fileInfo.DirectoryName ?? "Null");
            info.text += "<b>Name:</b> " + fileInfo.Name + "\n"
                + "<b>Extension:</b> " + fileInfo.Extension + "\n"
                + "<b>Directory name:</b> " + directoryInfo.Name + "\n"
                + "<b>Creation time:</b> " + fileInfo.CreationTime.ToLongDateString() + " " + fileInfo.CreationTime.ToLongTimeString() + "\n"
                + "<b>Last access time:</b> " + fileInfo.LastAccessTime.ToLongTimeString() + "\n"
                + "<b>Full path:</b> " + ExplorerItem.Info.path;
        }

        /// <summary>
        /// A function that performs a close action.
        /// </summary>
        private void Back()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// A function that performs the action of opening an item.
        /// </summary>
        private void Open()
        {
            if(ExplorerItem == null) return;
            ExplorerItem.Open();
        }

        /// <summary>
        /// A function that performs the share action.
        /// </summary>
        private void Share()
        {
            if(ExplorerItem == null) return;
            ExplorerItem.Share();
        }

        /// <summary>
        /// A function that performs the Save Item action.
        /// </summary>
        private void Save()
        {
            if(ExplorerItem == null) return;
            ExplorerItem.Save();
        }

        /// <summary>
        /// A function that performs the delete item action.
        /// </summary>
        private void Delete()
        {
            if(ExplorerItem == null) return;
            ExplorerItem.Delete();
            gameObject.SetActive(false);
        }
    }
}
