using System;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Utilities.FileManager
{
    /// <summary>
    /// The target storage type determines the location from which files will be parsed.
    /// </summary>
    [Serializable]
    public enum FM_StorageType
    {
        PersistentData = 0,
        Data = 1,
        StreamingAssets = 2,
        TemporaryCache = 4,
        Custom = 8
    }
    
    public class FileManager : MonoBehaviour
    {
        [Tooltip("File Explorer is a script that performs file recognition and search functions for the selected storage type.")]
        [SerializeField] private FileExplorer fileExplorer;
        [Tooltip("The target storage type determines the location from which files will be parsed.")]
        [SerializeField] private FM_StorageType storageType = FM_StorageType.PersistentData;
        [Tooltip("Custom path for file analysis and storage type. The parameter will be effective if the Storage type is set to Custom.")]
        [SerializeField] private string customStorageType = "ENTER THE PATH IF THE STORAGE TYPE IS CUSTOM";
        
        [SerializeField] private Button buttonOpenFileExplorer;
        [SerializeField] private Button buttonCloseFileExplorer;

        private void Start()
        {
            buttonOpenFileExplorer.onClick.AddListener(OpenFileExplorer);
            buttonCloseFileExplorer.onClick.AddListener(CloseFileExplorer);

            if (fileExplorer == null)
            {
                Debug.LogWarning("File Explorer not found!");
                return;
            }
            
            switch (storageType)
            {
                case FM_StorageType.PersistentData:
                    fileExplorer.TargetPath = Application.persistentDataPath;
                    break;
                case FM_StorageType.Data:
                    fileExplorer.TargetPath = Application.dataPath;
                    break;
                case FM_StorageType.StreamingAssets:
                    fileExplorer.TargetPath = Application.streamingAssetsPath;
                    break;
                case FM_StorageType.TemporaryCache:
                    fileExplorer.TargetPath = Application.temporaryCachePath;
                    break;
                case FM_StorageType.Custom:
                    fileExplorer.TargetPath = customStorageType;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// A function that performs the action of opening File Explorer.
        /// </summary>
        private void OpenFileExplorer()
        {
            if (fileExplorer == null)
            {
                Debug.LogWarning("File Explorer not found!");
                return;
            }
            
            fileExplorer.gameObject.SetActive(true);
        }

        /// <summary>
        /// A function that performs the action of closing File Explorer.
        /// </summary>
        private void CloseFileExplorer()
        {
            if (fileExplorer == null)
            {
                Debug.LogWarning("File Explorer not found!");
                return;
            }
            
            fileExplorer.gameObject.SetActive(false);
        }
    }
}
