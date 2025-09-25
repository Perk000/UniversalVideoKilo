using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    public static class GraphicSubsystemExtension
    {
        public static GraphicSubsystem GetGraphicSubsystemByID(this List<GraphicSubsystem> list, string id)
        {
            if (list == null) return null;
            if (list.Count == 0) return null;

            return list.Find(x => x.Id == id);
        }
        
        public static GraphicSubsystem GetGraphicSubsystemByID(this GraphicProvider provider, string id)
        {
            if (provider.GraphicSubsystems == null) return null;
            if (provider.GraphicSubsystems.Count == 0) return null;

            return provider.GraphicSubsystems.Find(x => x.Id == id);
        }
        
        public static string[] GetAllSavedFiles(this GraphicProvider provider)
        {
            // Check if there are any graphics subsystems.
            if(provider.GraphicSubsystems.Count == 0) return null;
    
            // Let's take the first subsystem.
            var graphicSubsystem = provider.GraphicSubsystems.First();
            var settings = graphicSubsystem.GraphicSettings;
    
            // We define the main path using the GraphicSettings of this subsystem.
            var path = settings.applicationDataPath switch
            {
                ApplicationDataPath.PersistentDataPath => Path.Combine(Application.persistentDataPath,
                    settings.filePath),
                ApplicationDataPath.DataPath => Path.Combine(Application.dataPath, settings.filePath),
                ApplicationDataPath.TemporaryCachePath => Path.Combine(Application.temporaryCachePath,
                    settings.filePath),
                _ => Path.Combine(Application.persistentDataPath, settings.filePath)
            };
    
            // Check for the target path.
            if (!Directory.Exists(path)) return null;
    
            return Directory.GetFiles(path, "*." + settings.encodeTo.ToString().ToLower(), SearchOption.AllDirectories);
        }
    }
}