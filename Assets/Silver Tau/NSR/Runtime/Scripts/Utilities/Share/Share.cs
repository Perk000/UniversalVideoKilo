using System;
using UnityEngine;
using System.IO;

#if !UNITY_EDITOR && PLATFORM_IOS
using System.Runtime.InteropServices;
using AOT;
#endif

#if !UNITY_EDITOR && PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Utilities
{
    public static class Share
    {
        #region Share dll
#if !UNITY_EDITOR && PLATFORM_IOS
        [DllImport("__Internal")]
        private static extern void NSR_Share(String path);
#endif
        
#if !UNITY_EDITOR && PLATFORM_IOS
        public delegate void NSR_Share_SuccessDelegate();
        [DllImport("__Internal")]
        private static extern void NSR_Share_Success(NSR_Share_SuccessDelegate callBack);

        public delegate void NSR_Share_ErrorDelegate(String value);
        [DllImport("__Internal")]
        private static extern void NSR_Share_Error(NSR_Share_ErrorDelegate callBack);
#endif
        
        #endregion
        
        #region Share
        
        /// <summary>
        /// A function that allows you to share a item.
        /// </summary>
        /// <param name="path">Path to the item (file, folder, any path).</param>
        public static void ShareItem(string path)
        {
            if(string.IsNullOrEmpty(path)) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            NSR_Share(path);
#elif !UNITY_EDITOR && PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            NSR_Android_Share(path);
#endif
        }
        
#if !UNITY_EDITOR && PLATFORM_IOS
        [MonoPInvokeCallback(typeof(NSR_Share_SuccessDelegate))]
        private static void DelegateNSR_Share_Success()
        {
            //recorderShare?.Invoke();
        }
        
        [MonoPInvokeCallback(typeof(NSR_Share_ErrorDelegate))]
        private static void DelegateNSR_Share_Error(String value)
        {
            //recorderShareError?.Invoke(value);
        }
#endif
        
        #endregion
        
#if !UNITY_EDITOR && PLATFORM_ANDROID
        public static void NSR_Android_Share(string path) {
            if (Application.platform == RuntimePlatform.Android) {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");      
                AndroidJavaObject share = new AndroidJavaObject("com.silvertau.nsrcorder.Share");           
                object[] parameters = new object[2];
                parameters[0] = unityActivity;
                parameters[1] = path;
                share.Call("ShareFile", parameters);
            }
        }
#endif
        
    }
}
