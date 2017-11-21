using System;

namespace CSharpSandbox.Host
{
	public class Program
	{
		public static void Main()
		{
			var clientApi = new ClientApi();
			
			var scriptsHost = new ScriptsHost(clientApi);
			scriptsHost.Initialize();
			scriptsHost.StartScripts();

			clientApi.RaiseEvent(nameof(ClientApi.SampleClientEvent), "Test argument.");

			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
