#if UNITY_EDITOR

#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using UnityEditor.iOS.Xcode;
using UnityEngine;

#endif

namespace SilverTau.NSR.Recorders
{
	public class NSPostProcessBuild 
	{
		private const bool ENABLED = true;
		private const string PHOTO_LIBRARY_USAGE_DESCRIPTION = "Save media to Photos";

#if UNITY_IOS
#pragma warning disable 0162
	[PostProcessBuild]
	public static void OnPostprocessBuild( BuildTarget target, string buildPath )
	{
		if( !ENABLED )
			return;

		if( target == BuildTarget.iOS )
		{
			string plistPath = Path.Combine( buildPath, "Info.plist" );

			PlistDocument plist = new PlistDocument();
			plist.ReadFromString( File.ReadAllText( plistPath ) );

			PlistElementDict rootDict = plist.root;
			rootDict.SetString( "NSPhotoLibraryUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );
			rootDict.SetString( "NSPhotoLibraryAddUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );

			File.WriteAllText( plistPath, plist.WriteToString() );
		}

		if (target == BuildTarget.Android)
		{
			string xmlPath = Path.Combine( buildPath, "AndroidManifest.xml" );

			XmlDocument doc = new XmlDocument();
			doc.Load(xmlPath);
			XmlElement activity = SearchActivity(doc);
			if (activity != null
			    && string.IsNullOrEmpty(activity.GetAttribute("android:hardwareAccelerated"))) {
				activity.SetAttribute("exported", "http://schemas.android.com/apk/res/android", "true");
				doc.Save(xmlPath);
				Debug.LogError("adjusted AndroidManifest.xml about android:exported. Please rebuild the app.");
			}
		}
	}
	
	private static XmlElement SearchActivity(XmlDocument doc) {
		foreach (XmlNode node0 in doc.DocumentElement.ChildNodes) {
			if (node0.Name == "application") {
				foreach (XmlNode node1 in node0.ChildNodes) {
#if UNITYWEBVIEW_ANDROID_USE_ACTIVITY_NAME
                    if (node1.Name == "activity"
                        && ((XmlElement)node1).GetAttribute("android:name") == ACTIVITY_NAME) {
                        return (XmlElement)node1;
                    }
#else
					if (node1.Name == "activity") {
						foreach (XmlNode node2 in node1.ChildNodes) {
							if (node2.Name == "intent-filter") {
								bool hasActionMain = false;
								bool hasCategoryLauncher = false;
								foreach (XmlNode node3 in node2.ChildNodes) {
									if (node3.Name == "action"
									    && ((XmlElement)node3).GetAttribute("android:name") == "android.intent.action.MAIN") {
										hasActionMain = true;
									} else if (node3.Name == "category"
									           && ((XmlElement)node3).GetAttribute("android:name") == "android.intent.category.LAUNCHER") {
										hasCategoryLauncher = true;
									}
								}
								if (hasActionMain && hasCategoryLauncher) {
									return (XmlElement)node1;
								}
							}
						}
#endif
					}
				}
			}
		}
		return null;
	}
	
#pragma warning restore 0162
#endif
	}
}

#endif