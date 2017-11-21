using System;
using System.IO;

namespace CSharpSandbox.ClientScript
{
	public class SomeClientScript : ClientScriptDomain.ClientScript
	{
		protected override void Loaded()
		{
			var filePath = Path.Combine(DataDirectoryPath, "test.txt");
			//filePath = "C:\\text.txt";
			File.WriteAllText(filePath, AppDomain.CurrentDomain.FriendlyName);

			ClientApi.GameTick += ClientApiOnGameTick;
		}

		private void ClientApiOnGameTick()
		{
			Console.WriteLine("Game tick");
		}
	}
}
