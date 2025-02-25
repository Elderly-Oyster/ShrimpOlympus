using System;
using CodeBase.Core.Systems.SaveSystem;

namespace CodeBase.Systems
{
	public class GraphicsSettingsSystem : ISerializableDataSystem
	{
		private int _loadingRange;

		public int LoadingRange
		{
			get => _loadingRange;
			set
			{
				if(value < 2)
					throw new ArgumentException("Loading Range cannot be less then 2");
				_loadingRange = value;
			}
		}

		public GraphicsSettingsSystem(SerializableDataSystemsManager systemsManager)
		{
			systemsManager.AddSystem(this);
		}

		public void Initialize(SerializableDataContainer dataContainer)
		{
			LoadingRange = dataContainer.TryGet(nameof(LoadingRange), out int loadingRange) ? loadingRange : 8;
		}

		public void WriteTo(SerializableDataContainer dataContainer)
		{
			dataContainer.SetData(nameof(LoadingRange), LoadingRange);
		}
	}
}