using System;
using System.Collections.Generic;
using SilverTau.NSR.Recorders.Audio;
using SilverTau.NSR.Recorders.Video;
using SilverTau.NSR.Recorders.Watermark;
using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR
{
    [CustomEditor(typeof(ScreenRecorder))]
    public class NSREditor : NSRPackageEditor
    {
        private ScreenRecorder _target;
        
        private void OnEnable()
        {
            if (target) _target = (ScreenRecorder)target;
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Screen Recorder</color></b>");
            
            base.OnInspectorGUI();
        }
        private static GameObject CreateElementRoot(string name)
        {
	        var child = new GameObject(name);
	        Undo.RegisterCreatedObjectUndo(child, "Create " + name);
	        Selection.activeGameObject = child;
	        return child;
        }

        static GameObject CreateObject(string name, GameObject parent)
        {
	        var go = new GameObject(name);
	        GameObjectUtility.SetParentAndAlign(go, parent);
	        return go;
        }
        
        [MenuItem("GameObject/Silver Tau/NSR/Screen Recorder (iOS)", false)]
		static public void AddNScreenRecorder()
		{
			var nsr = CreateElementRoot("NSR - Screen Recorder (iOS)");
			nsr.AddComponent<ScreenRecorder>();
		}
        
		[MenuItem("GameObject/Silver Tau/NSR/Universal Video Recorder", false)]
		static public void AddNUniversalScreenRecorder()
		{
			var nsr = CreateElementRoot("NSR - Video Recorder (Universal)");
			nsr.AddComponent<UniversalVideoRecorder>();
		}
        
		[MenuItem("GameObject/Silver Tau/NSR/Watermark (Universal Video Recorder)", false)]
		static public void AddNWatermarkNSR()
		{
			var nsr = CreateElementRoot("NSR - Watermark");
			var nSRWatermark = nsr.AddComponent<NSRWatermark>();

			var shader = Shader.Find("Silver Tau/NSR - Screen Recorder/Watermark/WatermarkOverlay");
			nSRWatermark.ChangeShader(shader);
		}
        
		[MenuItem("GameObject/Silver Tau/NSR/Microphone Audio Recorder", false)]
		static public void AddNMicrophoneAudioRecorder()
		{
			var nsr = CreateElementRoot("Microphone Audio Recorder (Universal)");
			nsr.AddComponent<MicrophoneAudioRecorder>();
		}
		
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Prepare and check the Screen Recorder (iOS)")]
		public static void PrepareAndCheckNSRScreenRecorderiOS()
		{
#if UNITY_2023_1_OR_NEWER
			var screenRecorder = UnityEngine.Object.FindFirstObjectByType<ScreenRecorder>();
#else
			var screenRecorder = UnityEngine.Object.FindObjectOfType<ScreenRecorder>();
#endif
			//var screenRecorder = UnityEngine.Object.FindObjectOfType<ScreenRecorder>();
			if (screenRecorder == null)
			{
				AddNScreenRecorder();
#if UNITY_2023_1_OR_NEWER
				screenRecorder = UnityEngine.Object.FindFirstObjectByType<ScreenRecorder>();
#else
				screenRecorder = UnityEngine.Object.FindObjectOfType<ScreenRecorder>();
#endif
			}
			
			if (screenRecorder == null) return;
			Debug.Log("<color=cyan>NSR - Screen Recorder - Screen Recorder (iOS) is ready to use.</color>");
		}

		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Prepare and check the Universal Video Recorder")]
		public static void PrepareAndCheckNSRScreenRecorder()
		{
#if UNITY_2023_1_OR_NEWER
			var universalVideoRecorder = UnityEngine.Object.FindFirstObjectByType<UniversalVideoRecorder>();
#else
			var universalVideoRecorder = UnityEngine.Object.FindObjectOfType<UniversalVideoRecorder>();
#endif
			if (universalVideoRecorder == null)
			{
				AddNUniversalScreenRecorder();
#if UNITY_2023_1_OR_NEWER
				universalVideoRecorder = UnityEngine.Object.FindFirstObjectByType<UniversalVideoRecorder>();
#else
				universalVideoRecorder = UnityEngine.Object.FindObjectOfType<UniversalVideoRecorder>();
#endif
			}
			
			if (universalVideoRecorder == null) return;
			Debug.Log("<color=cyan>NSR - Screen Recorder - Universal Video Recorder is ready to use.</color>");
		}
		
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Prepare and check the Microphone Audio Recorder (Universal)")]
		public static void PrepareAndCheckNSRMicrophoneAudioRecorder()
		{
#if UNITY_2023_1_OR_NEWER
			var microphoneAudioRecorder = UnityEngine.Object.FindFirstObjectByType<MicrophoneAudioRecorder>();
#else
			var microphoneAudioRecorder = UnityEngine.Object.FindObjectOfType<MicrophoneAudioRecorder>();
#endif
			if (microphoneAudioRecorder == null)
			{
				AddNMicrophoneAudioRecorder();
#if UNITY_2023_1_OR_NEWER
				microphoneAudioRecorder = UnityEngine.Object.FindFirstObjectByType<MicrophoneAudioRecorder>();
#else
				microphoneAudioRecorder = UnityEngine.Object.FindObjectOfType<MicrophoneAudioRecorder>();
#endif
			}
			
			if (microphoneAudioRecorder == null) return;
			Debug.Log("<color=cyan>NSR - Screen Recorder - Microphone Audio Recorder (Universal) is ready to use.</color>");
		}
        
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Validate plugin", false, 1)]
		public static void ValidateNSRScreenRecorder()
		{
			var hasWarning = false;
			
#if UNITY_2023_1_OR_NEWER
			var universalVideoRecorder = UnityEngine.Object.FindObjectsByType<UniversalVideoRecorder>(FindObjectsSortMode.None);
#else
			var universalVideoRecorder = UnityEngine.Object.FindObjectsOfType<UniversalVideoRecorder>();
#endif

			if (universalVideoRecorder.Length == 0)
			{
				Debug.LogWarning("There is no Universal Video Recorder component on the scene opened in the editor.");
				
#if UNITY_2023_1_OR_NEWER
				var screenRecorder = UnityEngine.Object.FindObjectsByType<ScreenRecorder>(FindObjectsSortMode.None);
#else
				var screenRecorder = UnityEngine.Object.FindObjectsOfType<ScreenRecorder>();
#endif
				
				if (screenRecorder.Length == 0)
				{
					Debug.LogWarning("There is no Screen Recorder (iOS) component on the scene opened in the editor.");
					hasWarning = true;
				}
				else if (screenRecorder.Length > 1)
				{
					Debug.LogWarning("There is more than one Screen Recorder (iOS) component in the scene open in the editor. There should be only one Screen Recorder (iOS) component on the scene.");
					hasWarning = true;
				}
				else
				{
					hasWarning = true;
				}
			}
			else if (universalVideoRecorder.Length > 1)
			{
				Debug.LogWarning("There is more than one Universal Video Recorder component in the scene open in the editor. There should be only one Universal Video Recorder component on the scene.");
				hasWarning = true;
			}
			
			var stcAssets = AssetDatabase.FindAssets("StcCorder");
			var nsrAssets = AssetDatabase.FindAssets("NSR");
			var assets = new List<string>();
			assets.AddRange(stcAssets);
			assets.AddRange(nsrAssets);

			var files = new List<string> {"StcCorder.framework", "StcCorder.dll", "StcCorder.bundle", "StcCorder.aar", "NSR.framework", "NSR.aar", "NSR_Core.dll", "NSR_Core_iOS.dll"};
			
			var refValues = new List<bool>();
			
			foreach (var asset in assets)
			{
				var path = AssetDatabase.GUIDToAssetPath(asset);
				CheckAssetInProject(path, files, "NSR.framework", ref refValues);
				CheckAssetInProject(path, files, "StcCorder.framework", ref refValues);
				CheckAssetInProject(path, files, "StcCorder.dll", ref refValues);
				CheckAssetInProject(path, files, "NSR.aar", ref refValues);
				CheckAssetInProject(path, files, "StcCorder.bundle", ref refValues);
				CheckAssetInProject(path, files, "StcCorder.aar", ref refValues);
				CheckAssetInProject(path, files, "NSR_Core.dll", ref refValues);
				CheckAssetInProject(path, files, "NSR_Core_iOS.dll", ref refValues);
			}

			if (refValues.Contains(true) || files.Count > 0)
			{
				foreach (var assetName in files)
				{
					Debug.LogWarning("Cannot find the <b>" + assetName + "</b> library in the project files.");
				}
				
				hasWarning = true;
			}
			
#if PLATFORM_IOS
			if (!ValidateTargetOsVersion())
			{
				Debug.LogWarning("The minimum recommended OS version should be 12+.");
				hasWarning = true;
			}
#endif
			if(!hasWarning) Debug.Log("<color=cyan>NSR - Screen Recorder is ready to use.</color>");
		}
		
#if PLATFORM_IOS
	    private static bool ValidateTargetOsVersion()
	    {
		    Version ver;
            
		    try
		    {
			    ver = Version.Parse(PlayerSettings.iOS.targetOSVersionString);
		    }
		    catch (Exception e)
		    {
			    Debug.Log(e);
			    return false;
		    }
            
		    return ver.Major >= 12;
	    }
#endif

	    private static void CheckAssetInProject(string path, ICollection<string> files, string assetName, ref List<bool> hasWarning)
	    {
		    if (!path.Contains(assetName)) return;
		    
		    if (path.Contains(assetName + ".meta")) return;
		    
		    if (files.Contains(assetName))
		    {
			    files.Remove(assetName);
		    }
		    else
		    {
			    Debug.LogWarning("More than one " +assetName +" was found in the project. There should be only one " + assetName + ".\nPath: " + path);
			    hasWarning.Add(true);
		    }
	    }
	    
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Documentation (Web)", false, 3)]
		public static void DocumentationWebNSRScreenRecorder()
		{
			Application.OpenURL("https://silvertau.s3.eu-central-1.amazonaws.com/NSR-ScreenRecorder/Documentation/index.html");
		}
	    
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Product Page", false, 2)]
		public static void ProductPageNSRScreenRecorder()
		{
			Application.OpenURL("https://www.silvertau.com/products/nsr-screen-recorder");
		}

		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Unity Asset Store Page", false, 3)]
		public static void UnityAssetStorePageWebNSRScreenRecorder()
		{
			Application.OpenURL("https://prf.hn/l/q5yDAXe");
		}
    }
}