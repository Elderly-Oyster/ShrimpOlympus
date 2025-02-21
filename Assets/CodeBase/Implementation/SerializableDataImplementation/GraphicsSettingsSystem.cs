using System;
using CodeBase.Core.SerializableDataCore;
using UnityEngine;
using VContainer.Unity;

namespace CodeBase.Implementation.SerializableDataImplementation
{
	public class GraphicsSettingsSystem : ISerializableDataSystem
	{
		private int _loadingRange = 0;

		public int LoadingRange
		{
			get => _loadingRange;
			set
			{
				if(value < 2)
				{
					throw new ArgumentException("Loading Range cannot be less then 2");
				}

				_loadingRange = value;
			}
		}

		public GraphicsSettingsSystem(SerializableDataSystemsManager systemsManager)
		{
			systemsManager.AddSystem(this);
		}

		public void Initialize(SerializableDataContainer dataContainer)
		{
			if(dataContainer.TryGet(nameof(LoadingRange), out int loadingRange))
			{
				LoadingRange = loadingRange;
			}
			else
			{
				LoadingRange = 8;
			}
		}

		public void WriteTo(SerializableDataContainer dataContainer)
		{
			dataContainer.Set(nameof(LoadingRange), LoadingRange);
		}
	}
}