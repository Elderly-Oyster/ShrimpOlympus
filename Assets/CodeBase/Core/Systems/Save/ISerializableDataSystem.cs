using System.Threading.Tasks;

namespace CodeBase.Core.Systems.Save
{
	public interface ISerializableDataSystem
	{
		/// <summary>
		/// Load own data from dataContainer
		/// </summary>
		public Task LoadData(SerializableDataContainer dataContainer);
		/// <summary>
		/// Save own data to dataContainer
		/// </summary>
		public void SaveData(SerializableDataContainer dataContainer);
	}
}