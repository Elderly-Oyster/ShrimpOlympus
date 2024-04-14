﻿using System;
using System.IO;
using Services.DataPersistenceSystem;
using UnityEngine;

namespace Core.Systems.DataPersistenceSystem
{
    public class FileDataHandler
    {
        private readonly string _dataDirPath;
        private readonly string _dataFileName;
        private readonly bool _useEncryption;
        private const string EncryptionCodeWord = "word";
        
        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
        {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }
        public GameData Load()
        {
            var fullPath = Path.Combine(_dataDirPath, _dataFileName);
            GameData loadedData = null;
            if (!File.Exists(fullPath)) return null;
            try
            {
                string dataToLoad;
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (_useEncryption) 
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to load data to file: " + fullPath + "\n" + e);
            }
            return loadedData;
        }
        public void Save(GameData data)
        {
            var fullPath = Path.Combine(_dataDirPath, _dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                var dataToStore = JsonUtility.ToJson(data, true);
                if (_useEncryption) dataToStore = EncryptDecrypt(dataToStore);
                
                using var stream = new FileStream(fullPath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.Write(dataToStore);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
        
        private string EncryptDecrypt(string data) 
        {
            var modifiedData = "";
            for (var i = 0; i < data.Length; i++) 
                modifiedData += (char) (data[i] ^ EncryptionCodeWord[i % EncryptionCodeWord.Length]);
            
            return modifiedData;
        }
    }
}