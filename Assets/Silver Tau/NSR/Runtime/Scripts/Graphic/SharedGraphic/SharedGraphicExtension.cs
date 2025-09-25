using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace SilverTau.NSR.Graphic
{
    public static class SharedGraphicExtension
    {
        public static void SaveToFile(this SharedGraphic sharedGraphic, 
                                             ApplicationDataPath applicationDataPath, 
                                             string filePath,
                                             EncodeTo fileFormat = EncodeTo.PNG,
                                             int quality = 95,
                                             bool asynchronous = true,
                                             System.Action<bool, string> successfully = null)
        {
            SaveSharedGraphicToFile(sharedGraphic, applicationDataPath, filePath, fileFormat, quality, asynchronous, successfully);
        }
        
        private static void SaveSharedGraphicToFile(SharedGraphic sharedGraphic,
                                             ApplicationDataPath applicationDataPath,
                                             string filePath,
                                             EncodeTo fileFormat = EncodeTo.PNG,
                                             int quality = 95,
                                             bool asynchronous = true,
                                             System.Action<bool, string> successfully = null) 
        {
            // check that the input we're getting is something we can handle:
            if (!(sharedGraphic.texture is Texture2D || sharedGraphic.texture is RenderTexture))
            {
                successfully?.Invoke(false, null);
                return;
            }
         
            // use the original texture size in case the input is negative:
            var width = sharedGraphic.texture.width;
            var height = sharedGraphic.texture.height;
         
            string path;

            switch (applicationDataPath)
            {
                case ApplicationDataPath.PersistentDataPath:
                    path = Path.Combine(Application.persistentDataPath, filePath);
                    break;
                case ApplicationDataPath.DataPath:
                    path = Path.Combine(Application.dataPath, filePath);
                    break;
                case ApplicationDataPath.TemporaryCachePath:
                    path = Path.Combine(Application.temporaryCachePath, filePath);
                    break;
                default:
                    path = Path.Combine(Application.persistentDataPath, filePath);
                    break;
            }
                
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var finalPath = string.Empty;
            
            // resize the original image:
            //var resizeRT = RenderTexture.GetTemporary(width, height, 0);
            //Graphics.Blit(source, resizeRT);
         
            // create a native array to receive data from the GPU:
            var nativeArray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
         
            // request the texture data back from the GPU:
            var request = AsyncGPUReadback.RequestIntoNativeArray (ref nativeArray, sharedGraphic.texture, 0, (AsyncGPUReadbackRequest request) =>
            {
                // if the readback was successful, encode and write the results to disk
                if (!request.hasError)
                {
                    NativeArray<byte> encoded;
         
                    switch (fileFormat)
                    {
                        case EncodeTo.EXR:
                            encoded = ImageConversion.EncodeNativeArrayToEXR(nativeArray, sharedGraphic.texture.graphicsFormat, (uint)width, (uint)height);
                            break;
                        case EncodeTo.JPG:
                            encoded = ImageConversion.EncodeNativeArrayToJPG(nativeArray, sharedGraphic.texture.graphicsFormat, (uint)width, (uint)height, 0, quality);
                            break;
                        case EncodeTo.PNG:
                            encoded = ImageConversion.EncodeNativeArrayToPNG(nativeArray, sharedGraphic.texture.graphicsFormat, (uint)width, (uint)height);
                            break;
                        case EncodeTo.TGA:
                            encoded = ImageConversion.EncodeNativeArrayToTGA(nativeArray, sharedGraphic.texture.graphicsFormat, (uint)width, (uint)height);
                            break;
                        default:
                            encoded = ImageConversion.EncodeNativeArrayToPNG(nativeArray, sharedGraphic.texture.graphicsFormat, (uint)width, (uint)height);
                            break;
                    }
                    
                    var fileName = (string.IsNullOrEmpty(sharedGraphic.name) ? sharedGraphic.id : sharedGraphic.name) + '.' + fileFormat.ToString().ToLower();
                    finalPath = Path.Combine(path, fileName);

                    sharedGraphic.outputPath = finalPath;
                    
                    System.IO.File.WriteAllBytes(finalPath, encoded.ToArray());

                    encoded.Dispose();
                }
         
                nativeArray.Dispose();
         
                // notify the user that the operation is done, and its outcome.
                successfully?.Invoke(!request.hasError, finalPath); 
            });
         
            if (!asynchronous) request.WaitForCompletion();
        }
    }
}

