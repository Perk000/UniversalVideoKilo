using System.IO;
using UnityEngine;

namespace SilverTau.NSR
{
    public static class OpenInFileBrowser
    {
        public static void OpenInMacOSFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;
            
            // try mac
            string macPath = path.Replace("\\", "/");

            if (Directory.Exists(macPath))
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }
            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }
            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
            
            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                e.HelpLink = "";
            }
        }

        public static void OpenInWindowsFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;

            string winPath = path.Replace("/", "\\");

            if (Directory.Exists(winPath))
            {
                openInsidesOfFolder = true;
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                e.HelpLink = "";
            }
        }

        public static void OpenFileBrowser(string path)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
#if UNITY_2021_2_OR_NEWER
                case RuntimePlatform.OSXServer:
#endif
                    OpenInMacOSFileBrowser(path);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
#if UNITY_2021_2_OR_NEWER
                case RuntimePlatform.WindowsServer:
#endif
                    OpenInWindowsFileBrowser(path);
                    break;
                default:
                    break;
            }
        }
    }
}
