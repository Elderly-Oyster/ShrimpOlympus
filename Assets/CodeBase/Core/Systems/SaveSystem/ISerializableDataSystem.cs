namespace CodeBase.Core.Systems.SaveSystem
{
	public interface ISerializableDataSystem
	{
		/// <summary>
		/// Load own data from dataContainer
		/// </summary>
		public void Initialize(SerializableDataContainer dataContainer);
		/// <summary>
		/// Save own data to dataContainer
		/// </summary>
		public void WriteTo(SerializableDataContainer dataContainer);
	}
}