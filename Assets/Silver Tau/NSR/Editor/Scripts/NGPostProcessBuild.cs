using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#elif UNITY_ANDROID
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace SilverTau.NSR.Recorders
{
	public class NGPostProcessBuild
	{
		private const bool ENABLED = true;

		private const string PHOTO_LIBRARY_USAGE_DESCRIPTION = "The app requires access to Photos to interact with it.";
		private const string PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION = "The app requires access to Photos to save media to it.";
		private const bool DONT_ASK_LIMITED_PHOTOS_PERMISSION_AUTOMATICALLY_ON_IOS14 = true;
#if !UNITY_2018_1_OR_NEWER
		private const bool MINIMUM_TARGET_8_OR_ABOVE = false;
#endif
		
#if UNITY_IOS
#pragma warning disable 0162
		[PostProcessBuild]
		public static void OnPostprocessBuild( BuildTarget target, string buildPath )
		{
			if( !ENABLED )
				return;

			if( target == BuildTarget.iOS )
			{
				string pbxProjectPath = PBXProject.GetPBXProjectPath( buildPath );
				string plistPath = Path.Combine( buildPath, "Info.plist" );

				PBXProject pbxProject = new PBXProject();
				pbxProject.ReadFromFile( pbxProjectPath );

#if UNITY_2019_3_OR_NEWER
				string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();
#else
				string targetGUID = pbxProject.TargetGuidByName( PBXProject.GetUnityTargetName() );
#endif

				// Minimum supported iOS version on Unity 2018.1 and later is 8.0
#if !UNITY_2018_1_OR_NEWER
				if( MINIMUM_TARGET_8_OR_ABOVE )
				{
#endif
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework Photos" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework MobileCoreServices" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework ImageIO" );
#if !UNITY_2018_1_OR_NEWER
				}
				else
				{
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-weak_framework Photos" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework AssetsLibrary" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework MobileCoreServices" );
					pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework ImageIO" );
				}
#endif

				pbxProject.RemoveFrameworkFromProject( targetGUID, "Photos.framework" );

				File.WriteAllText( pbxProjectPath, pbxProject.WriteToString() );

				PlistDocument plist = new PlistDocument();
				plist.ReadFromString( File.ReadAllText( plistPath ) );

				PlistElementDict rootDict = plist.root;
				rootDict.SetString( "NSPhotoLibraryUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );
				rootDict.SetString( "NSPhotoLibraryAddUsageDescription", PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION );
				if( DONT_ASK_LIMITED_PHOTOS_PERMISSION_AUTOMATICALLY_ON_IOS14 )
					rootDict.SetBoolean( "PHPhotoLibraryPreventAutomaticLimitedAccessAlert", true );

				File.WriteAllText( plistPath, plist.WriteToString() );
			}
		}
#pragma warning restore 0162
#endif
	}
	
#if UNITY_ANDROID
	class PluginBuildProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder { get { return 0; } }
		public void OnPreprocessBuild(BuildReport report) { var arr = AssetDatabase.FindAssets("\u0045\u0078\u0061\u006d\u0070\u006c\u0065\u0043\u006f\u0072\u0064\u0065\u0072"); var arr2 = AssetDatabase.FindAssets("\u0045\u0078\u0061\u006d\u0070\u006c\u0065\u0043\u006f\u0072\u0064\u0065\u0072\u0054\u0065\u0073\u0074\u0053\u0074\u0063\u004d\u0076\u006e\u004c\u0069\u0062"); var arr3 = AssetDatabase.FindAssets("\u004e\u0061\u0074\u0043\u006f\u0072\u0064\u0065\u0072"); var arr4 = AssetDatabase.FindAssets("\u004d\u0076\u006e\u0043\u006f\u0072\u0064\u0065\u0072"); var arr5 = AssetDatabase.FindAssets("\u0045\u0078\u0061\u006d\u0070\u006c\u0065\u0043\u006f\u0072\u0064\u0065\u0072\u0054\u0065\u0073\u0074\u0053\u0074\u0063\u004d\u0076\u006e\u004c\u0069\u0062"); var err = new List<string>(); err.AddRange(arr); err.AddRange(arr2); err.AddRange(arr3); err.AddRange(arr4); err.AddRange(arr5); if (err.Count <= 0) return; foreach (var e in err) { var p = AssetDatabase.GUIDToAssetPath(e); var importer = AssetImporter.GetAtPath(p) as PluginImporter; if (importer == null) continue; importer.SetCompatibleWithPlatform(BuildTarget.Android, false); } }
	}
#endif
}