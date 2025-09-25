using System;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

#if !UNITY_EDITOR && PLATFORM_IOS
using System.Runtime.InteropServices;
using AOT;
#endif

#if !UNITY_EDITOR && PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Utilities
{
    public static class Gallery
    {
        #region Gallery dll
#if !UNITY_EDITOR && PLATFORM_IOS
        [DllImport("__Internal")]
        private static extern void NSR_SaveVideoToPhotosAlbum(String path);
        
        [DllImport("__Internal")]
        private static extern void NSR_SaveImageToPhotosAlbum(String path);
#endif
        
#if !UNITY_EDITOR && PLATFORM_IOS
        public delegate void NSR_SaveImageToGalleryDelegate(bool result, String error);
        [DllImport("__Internal")]
        private static extern void NSR_SaveImageToGallery_Delegate(NSR_SaveImageToGalleryDelegate callBack);

        public delegate void NSR_SaveVideoToGalleryDelegate(bool result, String error);
        [DllImport("__Internal")]
        private static extern void NSR_SaveVideoToGallery_Delegate(NSR_SaveVideoToGalleryDelegate callBack);
#endif
        #endregion
        
        #region Gallery

        /// <summary>
        /// An action that is called when the process of saving an image to the gallery is complete.
        /// </summary>
        public static UnityAction<bool, string> onSaveImageToGallery;
        
        /// <summary>
        /// An action that is called when the process of saving a video to the gallery is complete.
        /// </summary>
        public static UnityAction<bool, string> onSaveVideoToGallery;
        
        /// <summary>
        /// A method that allows you to save a video file to Gallery (Photos Album).
        /// </summary>
        /// <param name="path">Path to video.</param>
        public static void SaveVideoToGallery(string path, string androidFolderPath = "NSR_Video")
        {
            if(string.IsNullOrEmpty(path)) return;
#if !UNITY_EDITOR && PLATFORM_IOS
            NSR_SaveVideoToGallery_Delegate(DelegateNSR_SaveVideoToGallery);
            NSR_SaveVideoToPhotosAlbum(path);
#elif !UNITY_EDITOR && PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            NSR_Android_SaveVideoToGallery(path, androidFolderPath);
#endif
        }
        
        /// <summary>
        /// A method that allows you to save an image file to Gallery (Photos Album).
        /// </summary>
        /// <param name="path">Path to image.</param>
        public static void SaveImageToGallery(string path, string androidFolderPath = "NSR_Video")
        {
            if(string.IsNullOrEmpty(path)) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            NSR_SaveImageToGallery_Delegate(DelegateNSR_SaveImageToGallery);
            NSR_SaveImageToPhotosAlbum(path);
#elif !UNITY_EDITOR && PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            NSR_Android_SaveImageToGallery(path, androidFolderPath);
#endif
        }
        
#if !UNITY_EDITOR && PLATFORM_IOS
        [MonoPInvokeCallback(typeof(NSR_SaveImageToGalleryDelegate))]
        private static void DelegateNSR_SaveImageToGallery(bool result, String error)
        {
            onSaveImageToGallery?.Invoke(result, error);
        }
        
        [MonoPInvokeCallback(typeof(NSR_SaveVideoToGalleryDelegate))]
        private static void DelegateNSR_SaveVideoToGallery(bool result, String error)
        {
            onSaveVideoToGallery?.Invoke(result, error);
        }
#endif
        
        
#if !UNITY_EDITOR && PLATFORM_ANDROID
        public static void NSR_Android_SaveImageToGallery(string path, string folderPath = "NSR_Video") {
            if (Application.platform == RuntimePlatform.Android) {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");      
                AndroidJavaObject gallery = new AndroidJavaObject("com.silvertau.nsrcorder.Gallery");           
                object[] parameters = new object[3];
                parameters[0] = unityActivity;
                parameters[1] = path;
                parameters[2] = folderPath;
                try
                {
                    gallery.Call("SaveImageToGallery", parameters);
                    onSaveImageToGallery?.Invoke(true, "");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    onSaveImageToGallery?.Invoke(true, e.ToString());
                }
            }
        }

        public static void NSR_Android_SaveVideoToGallery(string path, string folderPath = "NSR_Video") {
            if (Application.platform == RuntimePlatform.Android) {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");      
                AndroidJavaObject gallery = new AndroidJavaObject("com.silvertau.nsrcorder.Gallery");           
                object[] parameters = new object[3];
                parameters[0] = unityActivity;
                parameters[1] = path;
                parameters[2] = folderPath;
                
                try
                {
                    gallery.Call("SaveVideoToGallery", parameters);
                    onSaveVideoToGallery?.Invoke(true, "");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    onSaveVideoToGallery?.Invoke(true, e.ToString());
                }
            }
        }
#endif
        
        #endregion
    }
}
