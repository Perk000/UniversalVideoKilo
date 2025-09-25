using System;
using System.Collections;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Helpers
{
    public static class PermissionHelper
    {
        public static IEnumerator CheckMicrophonePermissions(Action<bool> result)
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.2f);
            }

            result?.Invoke(Permission.HasUserAuthorizedPermission(Permission.Microphone));
            yield break;
#elif PLATFORM_IOS// || UNITY_WEBGL
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
            
#if !NSR_MICROPHONE_DISABLE
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                try
                {
                    Microphone.Start(null, false, 1, AudioSettings.outputSampleRate);
                    Microphone.End(null);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
#endif
            
            result?.Invoke(Application.HasUserAuthorization(UserAuthorization.Microphone));
            yield break;
#elif PLATFORM_STANDALONE
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
            
#if !NSR_MICROPHONE_DISABLE
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                try
                {
                    Microphone.Start(null, false, 1, AudioSettings.outputSampleRate);
                    Microphone.End(null);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
#endif
            
            result?.Invoke(Application.HasUserAuthorization(UserAuthorization.Microphone));
            yield break;
#else
            result?.Invoke(false);
            yield break;
#endif
        }
        
        public static IEnumerator CheckCameraPermissions(Action<bool> result)
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.2f);
            }

            result?.Invoke(Permission.HasUserAuthorizedPermission(Permission.Camera));
            yield break;
#elif PLATFORM_IOS// || UNITY_WEBGL
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
            
            result?.Invoke(Application.HasUserAuthorization(UserAuthorization.WebCam));
            yield break;
#elif PLATFORM_STANDALONE
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
            
            result?.Invoke(Application.HasUserAuthorization(UserAuthorization.WebCam));
            yield break;
#else
            result?.Invoke(false);
            yield break;
#endif
        }
    }
}