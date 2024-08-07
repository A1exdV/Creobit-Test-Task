using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoad
{
    public class DesktopSaveLoad : ISaveLoad
    {
        private const string FileFormat = ".json";
        private const string FileName = "SaveData";
        private const string SaveFolder = "Saves";
        
        private const string CustomPathInEditor = "Assets/Saves";

        public SaveData SaveData { get; set; }

        public DesktopSaveLoad()
        {
            Load();
        }

        public void Save()
        {
            try
            {
                var filePath = GetFilePath();

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath ??
                                              throw new InvalidOperationException("Directory path is null."));
                }

                var jsonData = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error when saving data: {ex.Message}");
            }
            finally
            {
                Debug.Log("Data has been saved successfully!");
            }
        }
        
        public void Load()
        {
            try
            {
                var filePath = GetFilePath();
                if (File.Exists(filePath))
                {
                    var jsonData = File.ReadAllText(filePath);
                    SaveData =  JsonConvert.DeserializeObject<SaveData>(jsonData);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error when loading data: {ex.Message}");
            }

            SaveData = new SaveData();
        }

        private static string GetFilePath()
        {
#if UNITY_EDITOR
            return Path.Combine(CustomPathInEditor, $"{FileName}{FileFormat}");
#else
            return Path.Combine(System.Environment.CurrentDirectory, SaveFolder, $"{FileName}{FileFormat}");
#endif
        }
    }
}