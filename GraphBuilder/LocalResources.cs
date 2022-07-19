namespace GraphBuilder
{
	public sealed class LocalResources
	{
		// use only one instane of the WpfResources class internally
		static readonly Resources _resources = new Resources();

		/// <summary>
		/// For internal use.
		/// </summary>
		public LocalResources()
		{
			// do not remove this constructor.
		}

		/// <summary>
		/// For internal use.
		/// </summary>
		public Resources Resources { get { return _resources; } }
	}
}
