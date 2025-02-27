using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CodeBase.Core.Systems.Save
{
	public class SerializableDataFileLoader
	{
		private const string FileName = "save.json";

		private readonly string _saveFilePath;

		public SerializableDataFileLoader()
		{
			_saveFilePath = Path.Combine(Application.persistentDataPath, FileName);
		}

		public async UniTask Write(SerializableDataContainer dataContainer) 
		{
			await UniTask.RunOnThreadPool(() => WriteInternal(dataContainer));
		}

		public UniTask<SerializableDataContainer> Read() 
		{
			return UniTask.RunOnThreadPool(ReadInternal);
		}

		private void WriteInternal(SerializableDataContainer dataContainer) 
		{
			var serializedData = JsonConvert.SerializeObject(dataContainer, Formatting.Indented);
			File.WriteAllText(_saveFilePath, serializedData);
		}

		private SerializableDataContainer ReadInternal() 
		{
			try
			{
				if(!File.Exists(_saveFilePath))
				{
					return null;
				}

				var serializedData = File.ReadAllText(_saveFilePath);
				return JsonConvert.DeserializeObject<SerializableDataContainer>(serializedData);
			}
			catch(Exception)
			{
				return null;
			}			
		}
	}
}