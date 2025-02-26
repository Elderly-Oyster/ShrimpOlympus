using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Systems.SaveSystem
{
	public class SerializableDataSystemsManager
	{
		private readonly List<ISerializableDataSystem> _serializableDataSystems = new();
		private readonly SerializableDataFileLoader _serializableDataFileLoader = new();

		private SerializableDataContainer _serializableDataContainer;
		private bool _isLoaded;

		public SerializableDataSystemsManager()
		{
			MonoBehaviourEventsHandler.OnApplicationFocusEvent += SaveDataOnApplicationUnfocus;
		}

		public async UniTaskVoid Initialize()
		{
			_serializableDataContainer = await _serializableDataFileLoader.Read();
			if(_serializableDataContainer == null)
			{
				_serializableDataContainer = new SerializableDataContainer();
			}

			foreach(var settingsSystem in _serializableDataSystems)
			{
				settingsSystem.Initialize(_serializableDataContainer);
			}

			_isLoaded = true;
		}

		public void AddSystem(ISerializableDataSystem serializableDataSystem) 
		{
			_serializableDataSystems.Add(serializableDataSystem);
			if(_isLoaded)
			{
				serializableDataSystem.Initialize(_serializableDataContainer);
			}
		}

		public async UniTaskVoid SaveData()
		{
			foreach(var serializableDataSystem in _serializableDataSystems)
			{
				serializableDataSystem.WriteTo(_serializableDataContainer);
			}

			await _serializableDataFileLoader.Write(_serializableDataContainer);
		}

		private void SaveDataOnApplicationUnfocus(bool isFocused)
		{
			if(!isFocused)
			{
				SaveData().Forget();
			}
		}
	}
}