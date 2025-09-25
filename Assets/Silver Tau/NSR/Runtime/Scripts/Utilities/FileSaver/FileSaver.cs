using System;
using UnityEngine;
using System.IO;

namespace SilverTau.NSR.Utilities
{
    /// <summary>
    /// Image encoding format.
    /// </summary>
    [Serializable]
    public enum TextureEncodeTo
    {
        EXR, JPG, PNG, TGA
    }
    
    public static class FileSaver
    {
        public static bool TrySaveTextureToFile(Texture2D texture, TextureEncodeTo encodeTo, out string resultPath)
        {
            resultPath = null;
            
            if (texture == null) return false;
            
            var path = CreateTempPath();
            
            byte[] bytes;
        
            switch (encodeTo)
            {
                case TextureEncodeTo.PNG:
                    bytes = texture.EncodeToPNG();
                    path += ".png";
                    break;
                case TextureEncodeTo.JPG:
                    bytes = texture.EncodeToJPG(75);
                    path += ".jpg";
                    break;
                case TextureEncodeTo.EXR:
                    bytes = texture.EncodeToEXR(Texture2D.EXRFlags.None);
                    path += ".exr";
                    break;
                case TextureEncodeTo.TGA:
                    bytes = texture.EncodeToTGA();
                    path += ".tga";
                    break;
                default:
                    resultPath = null;
                    return false;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllBytes(path, bytes);
            
#if UNITY_EDITOR
            //Debug.Log("Texture saved to " + path);
#endif
            
            resultPath = path;
            return true;
        }
        
        public static bool TrySaveBytesToFile(byte[] data, string format, out string resultPath)
        {
            resultPath = null;
            if (data == null) return false;
            
            var path = CreateTempPath();

            if (!format.StartsWith("."))
            {
                format += ".";
            }
            
            path += format;

            File.WriteAllBytes(path, data);
            
#if UNITY_EDITOR
            //Debug.Log("Data saved to " + path);
#endif
            
            resultPath = path;
            return true;
        }

        private static string CreateTempPath()
        {
            var mainPath = Path.Combine(Application.temporaryCachePath, "NSR_FileSaver");

            if (!Directory.Exists(mainPath))
            {
                Directory.CreateDirectory(mainPath);
            }
            
            var fileName = "File_" + DateTime.Now.ToString("MMddyyyyHHmmss000");
            var finalPath = Path.Combine(mainPath, fileName);

            return finalPath;
        }
    }
}