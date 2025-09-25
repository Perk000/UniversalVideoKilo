using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    /// <summary>
    /// Class of display settings for elements.
    /// </summary>
    [Serializable]
    public class FE_InfoSettings
    {
        public ExplorerItemType type;
        public Sprite icon;
    }
    
    public class FileExplorer : MonoBehaviour
    {
        [Tooltip("A list of display settings for items.")]
        [SerializeField] private List<FE_InfoSettings> infoSettings = new List<FE_InfoSettings>();
        
        [Tooltip("Container for elements.")]
        [SerializeField] private Transform containerExplorerItems;
        [Tooltip("Element prefab.")]
        [SerializeField] private ExplorerItem prefabExplorerItem;
        [Tooltip("Information menu of the ExplorerItem item.")]
        [SerializeField] private ExplorerItemInfoMenu explorerItemInfoMenu;
        [Tooltip("Back button.")]
        [SerializeField] private Button buttonBack;
        
        /// <summary>
        /// The input target path.
        /// </summary>
        public string TargetPath { get; set; }
        
        /// <summary>
        /// A list of created items.
        /// </summary>
        public List<ExplorerItem> ExplorerItems { get; set; } = new List<ExplorerItem>();

        /// <summary>
        /// An open path.
        /// </summary>
        private string _currentTargetPath;
        
        /// <summary>
        /// A list of open paths.
        /// </summary>
        private List<string> _listPaths = new List<string>();
        
        private void Start()
        {
            buttonBack.onClick.AddListener(Back);
        }

        private void OnEnable()
        {
            Dispose();
            Refresh();
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
            ClearExplorerItems();
            _listPaths = new List<string>();
            ExplorerItems = new List<ExplorerItem>();
            _currentTargetPath = TargetPath;
            buttonBack.gameObject.SetActive(false);
        }

        /// <summary>
        /// A function that clears the list of items.
        /// </summary>
        private void ClearExplorerItems()
        {
            if (ExplorerItems.Count <= 0) return;
            
            foreach (var explorerItem in ExplorerItems)
            {
                Destroy(explorerItem.gameObject);
            }
            
            ExplorerItems = new List<ExplorerItem>();
        }

        /// <summary>
        /// A function that performs an action to update files in the target folder.
        /// </summary>
        public void Refresh()
        {
            if (!Directory.Exists(_currentTargetPath))
            {
                Debug.LogWarning("The target path is not valid!");
                return;
            }
            
            ClearExplorerItems();

            var directories = Directory.GetDirectories(_currentTargetPath);
            var files = Directory.GetFiles(_currentTargetPath, "*.*", SearchOption.TopDirectoryOnly);

            CreateItems(directories);
            CreateItems(files);
        }

        /// <summary>
        /// A function that performs the action of creating an item depending on the file type.
        /// </summary>
        /// <param name="array">Input array.</param>
        private void CreateItems(IEnumerable<string> array)
        {
            foreach (var element in array)
            {
                if(string.IsNullOrEmpty(element)) continue;
                
                var elementName = string.Empty;
                var explorerItemType = ExplorerItemType.None;
                
                if (element.EndsWith(".png") || element.EndsWith(".jpg") || element.EndsWith(".jpeg") ||
                    element.EndsWith(".tga") || element.EndsWith(".exr"))
                {
                    elementName = Path.GetFileName(element);
                    explorerItemType = ExplorerItemType.Image;
                }
                else if (element.EndsWith(".mp4") || element.EndsWith(".webm") || element.EndsWith(".avi"))
                {
                    elementName = Path.GetFileName(element);
                    explorerItemType = ExplorerItemType.Video;
                }
                else if (element.EndsWith(".wav") || element.EndsWith(".mp3"))
                {
                    elementName = Path.GetFileName(element);
                    explorerItemType = ExplorerItemType.Audio;
                }
                else if (element.EndsWith(".DS_Store") || element.EndsWith(".meta") || element.EndsWith(".Meta"))
                {
                    elementName = Path.GetFileName(element);
                    explorerItemType = ExplorerItemType.None;
                }
                else
                {
                    if (Directory.Exists(element))
                    {
                        var directoryInfo = new DirectoryInfo(element);
                        
                        elementName = directoryInfo.Name;
                        explorerItemType = ExplorerItemType.Folder;
                    }
                    else
                    {
                        elementName = Path.GetFileName(element);
                        explorerItemType = ExplorerItemType.File;
                    }
                }
                
                if(explorerItemType == ExplorerItemType.None) continue;
                
                var explorerItemIcon = infoSettings.Find(i => i.type == explorerItemType).icon;
                
                var explorerItemInfo = new ExplorerItemInfo
                {
                    name = elementName,
                    path = element,
                    type = explorerItemType,
                    icon = explorerItemIcon
                };

                var explorerItem = Instantiate(prefabExplorerItem, containerExplorerItems);
                explorerItem.Info = explorerItemInfo;
                explorerItem.buttonAction.onClick.AddListener(() => SelectExplorerItem(explorerItem));
                explorerItem.RefreshAction = Refresh;
                
                ExplorerItems.Add(explorerItem);
                
                explorerItem.Init();
            }
        }

        /// <summary>
        /// A function that returns to the previous folder.
        /// </summary>
        private void Back()
        {
            if (_listPaths.Count > 0)
            {
                _listPaths.RemoveAt(_listPaths.Count - 1);
            }
            
            if (_listPaths.Count == 0)
            {
                _currentTargetPath = TargetPath;
                buttonBack.gameObject.SetActive(false);
                Refresh();
                return;
            }

            _currentTargetPath = _listPaths.Last();
            Refresh();
        }

        /// <summary>
        /// A function that performs an item selection action.
        /// </summary>
        /// <param name="explorerItem">Explorer item.</param>
        private void SelectExplorerItem(ExplorerItem explorerItem)
        {
            if(explorerItem == null) return;
            if(explorerItem.Info == null) return;

            if (explorerItem.Info.type == ExplorerItemType.Folder)
            {
                _listPaths.Add(explorerItem.Info.path);
                _currentTargetPath = explorerItem.Info.path;
                buttonBack.gameObject.SetActive(true);
                Refresh();
                return;
            }

            explorerItemInfoMenu.ExplorerItem = explorerItem;
            explorerItemInfoMenu.gameObject.SetActive(true);
        }
    }
}
