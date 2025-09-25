using System;
using System.Collections;
using System.IO;
using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    /// <summary>
    /// The type of element.
    /// </summary>
    [Serializable]
    public enum ExplorerItemType
    {
        None = 0,
        Folder = 1,
        File = 2,
        Video = 4,
        Image = 8,
        Audio = 16
    }
    
    /// <summary>
    /// The class for the element information.
    /// </summary>
    [Serializable]
    public class ExplorerItemInfo
    {
        public string name;
        public string path;
        public ExplorerItemType type;
        public Sprite icon;
    }
    
    public class ExplorerItem : MonoBehaviour, IExplorerItem
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text title;
        [SerializeField] private ImageViewer prefabImageViewer;
        [SerializeField] private AudioViewer prefabAudioViewer;
        
        public Button buttonAction;
        public Action RefreshAction { get; set; }
        private IExplorerItem _explorerItemImplementation;

        /// <summary>
        /// The information element.
        /// </summary>
        public ExplorerItemInfo Info { get; set; }
        
        private void Start()
        {
            
        }

        /// <summary>
        /// The function that performs the action of initializing the information element.
        /// </summary>
        public void Init()
        {
            if(Info == null) return;
            icon.sprite = Info.icon;
            title.text = Info.name;
        }
        
        /// <summary>
        /// A function that performs the action of opening an item.
        /// </summary>
        public void Open()
        {
            if(Info == null) return;
          
            switch (Info.type)
            {
                case ExplorerItemType.None:
                case ExplorerItemType.Folder:
                case ExplorerItemType.File:
                    break;
                case ExplorerItemType.Video:
#if PLATFORM_STANDALONE || UNITY_EDITOR
                    OpenInFileBrowser(Info.path);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                    StartCoroutine(PlayStreamingVideo(Info.path));
#endif
                    break;
                case ExplorerItemType.Image:
                    if(prefabImageViewer != null){
                        var imageViewer = Instantiate(prefabImageViewer);
                        imageViewer.targetPath = Info.path;
                        imageViewer.Init();
                    }
                    break;
                case ExplorerItemType.Audio:
                    if(prefabImageViewer != null){
                        var imageViewer = Instantiate(prefabAudioViewer);
                        imageViewer.targetPath = Info.path;
                        imageViewer.Init();
                    }
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// A function that performs the share action.
        /// </summary>
        public void Share()
        {
            if(Info == null) return;
            
#if PLATFORM_STANDALONE || UNITY_EDITOR
            OpenInFileBrowser(Info.path);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            switch (Info.type)
            {
                case ExplorerItemType.None:
                    break;
                case ExplorerItemType.Folder:
                case ExplorerItemType.File:
                case ExplorerItemType.Video:
                case ExplorerItemType.Image:
                case ExplorerItemType.Audio:
                    Utilities.Share.ShareItem(Info.path);
                    break;
                default:
                    break;
            }
#endif
        }

        /// <summary>
        /// A function that performs the Save Item action.
        /// </summary>
        public void Save()
        {
            if(Info == null) return;
            
#if PLATFORM_STANDALONE || UNITY_EDITOR
            OpenInFileBrowser(Info.path);
#elif PLATFORM_IOS || PLATFORM_ANDROID
            switch (Info.type)
            {
                case ExplorerItemType.None:
                case ExplorerItemType.Folder:
                case ExplorerItemType.Audio:
                case ExplorerItemType.File:
                    break;
                case ExplorerItemType.Video:
                    Utilities.Gallery.SaveVideoToGallery(Info.path);
                    break;
                case ExplorerItemType.Image:
                    Utilities.Gallery.SaveImageToGallery(Info.path);
                    break;
                default:
                    break;
            }
#endif
        }

        /// <summary>
        /// A function that performs the delete item action.
        /// </summary>
        public void Delete()
        {
            if(Info == null) return;
            if(!File.Exists(Info.path)) return;
            File.Delete(Info.path);
            RefreshAction?.Invoke();
        }
        
        /// <summary>
        /// Coroutine that performs the preview function.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayStreamingVideo(string path)
        {
#if !UNITY_STANDALONE && !UNITY_EDITOR && !PLATFORM_WEBGL
            Handheld.PlayFullScreenMovie($"file://{path}");
#endif
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
        }

        private void OpenInFileBrowser(string path)
        {
#if PLATFORM_STANDALONE || UNITY_EDITOR
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(path);
#endif
        }
    }
}
