using System;
using CSharpSandbox.ClientSharedApi;

namespace CSharpSandbox.ClientScriptDomain
{
	public abstract class ClientScript : MarshalByRefObject
	{
		public string DataDirectoryPath { get; internal set; }

		public ClientApi ClientApi { get; internal set; }

		internal void CallLoaded()
		{
			Loaded();
		}
		
		protected abstract void Loaded();
	}
}