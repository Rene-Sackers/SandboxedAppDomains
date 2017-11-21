using System;

namespace CSharpSandbox.ClientSharedApi
{
	public abstract class ClientScript : MarshalByRefObject
	{
		public string DataDirectoryPath { get; internal set; }

		public IClientApi ClientApi { get; internal set; }

		internal void CallLoaded()
		{
			Loaded();
		}
		
		protected abstract void Loaded();
	}
}