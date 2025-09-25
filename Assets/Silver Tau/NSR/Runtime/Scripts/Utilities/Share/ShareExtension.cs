using System.IO;
using UnityEngine;

namespace SilverTau.NSR.Utilities
{
    public static class ShareExtension
    {
        public static void Share(this Texture2D texture2D, TextureEncodeTo textureEncodeTo)
        {
            if (FileSaver.TrySaveTextureToFile(texture2D, textureEncodeTo, out var path))
            {
                if(!File.Exists(path)) return;
                
                Utilities.Share.ShareItem(path);
                return;
            }
        }
        
        public static void Share(this byte[] bytes, string format)
        {
            if (FileSaver.TrySaveBytesToFile(bytes, format, out var path))
            {
                if(!File.Exists(path)) return;
                
                Utilities.Share.ShareItem(path);
                return;
            }
        }
        
        public static bool TryShareItem(Texture2D texture2D, TextureEncodeTo textureEncodeTo)
        {
            if (FileSaver.TrySaveTextureToFile(texture2D, textureEncodeTo, out var path))
            {
                if(!File.Exists(path)) return false;
                
                Utilities.Share.ShareItem(path);
                return true;
            }
            
            return false;
        }
        
        public static bool TryShareItem(byte[] bytes, string format)
        {
            if (FileSaver.TrySaveBytesToFile(bytes, format, out var path))
            {
                if(!File.Exists(path)) return false;
                
                Utilities.Share.ShareItem(path);
                return true;
            }
            
            return false;
        }
    }
}