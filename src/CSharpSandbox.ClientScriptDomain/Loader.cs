using System;
using System.Linq;
using System.Security;
using CSharpSandbox.ClientSharedApi;

namespace CSharpSandbox.ClientScriptDomain
{
	[SecurityCritical]
	internal class Loader : MarshalByRefObject
	{
		public void LoadClientScripts(string dataDirectoryPath, ClientApi clientApi)
		{
			var allTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.ToList();

			var scriptTypes = allTypes.Where(type => typeof(ClientScript).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
				.ToList();

			foreach (var scriptType in scriptTypes)
			{
				var clientScript = (ClientScript)Activator.CreateInstance(scriptType);
				clientScript.DataDirectoryPath = dataDirectoryPath;
				clientScript.ClientApi = clientApi;
				clientScript.CallLoaded();
			}
		}
	}
}