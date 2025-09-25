using System.IO;
using UnityEngine;

namespace SilverTau.NSR.Utilities
{
    public static class GalleryExtension
    {
        public static void SaveImageToGallery(this Texture2D texture2D, TextureEncodeTo textureEncodeTo, string androidFolderPath = "NSR_Video")
        {
            if (FileSaver.TrySaveTextureToFile(texture2D, textureEncodeTo, out var path))
            {
                if(!File.Exists(path)) return;
                
                Utilities.Gallery.SaveImageToGallery(path, androidFolderPath);
                return;
            }
        }
        
        public static bool TrySaveImageToGallery(Texture2D texture2D, TextureEncodeTo textureEncodeTo, string androidFolderPath = "NSR_Video")
        {
            if (FileSaver.TrySaveTextureToFile(texture2D, textureEncodeTo, out var path))
            {
                if(!File.Exists(path)) return false;
                
                Utilities.Gallery.SaveImageToGallery(path, androidFolderPath);
                return true;
            }
            
            return false;
        }
        
        public static void SaveVideoToGallery(this byte[] bytes, string format, string androidFolderPath = "NSR_Video")
        {
            if (FileSaver.TrySaveBytesToFile(bytes, format, out var path))
            {
                if(!File.Exists(path)) return;
                
                Utilities.Gallery.SaveVideoToGallery(path, androidFolderPath);
                return;
            }
        }
        
        public static bool TrySaveVideoToGallery(byte[] bytes, string format, string androidFolderPath = "NSR_Video")
        {
            if (FileSaver.TrySaveBytesToFile(bytes, format, out var path))
            {
                if(!File.Exists(path)) return false;
                
                Utilities.Gallery.SaveVideoToGallery(path, androidFolderPath);
                return true;
            }
            
            return false;
        }
    }
}