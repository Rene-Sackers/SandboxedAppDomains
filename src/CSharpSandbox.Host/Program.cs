using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpSandbox.ClientSharedApi;

namespace CSharpSandbox.Host
{
	public class Program
	{
		public static void Main()
		{
			var clientApi = new ClientApi();

			clientApi.Test = "a";

			var scriptsHost = new ScriptsHost(clientApi);
			scriptsHost.Initialize();
			scriptsHost.StartScripts();

			StartTicker(clientApi);

			Console.WriteLine("Done");
			Console.ReadKey();
		}

		private static void StartTicker(ClientApi clientApi)
		{
			Task.Run(async () =>
			{
				while (true)
				{
					await Task.Delay(1000);
					clientApi.RaiseEvent(nameof(ClientApi.GameTick));
				}
			});
		}
	}
}
