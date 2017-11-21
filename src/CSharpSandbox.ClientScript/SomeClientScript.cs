using System;
using System.IO;
using System.Reflection;

namespace CSharpSandbox.ClientScript
{
	public class SomeClientScript : ClientScriptDomain.ClientScript
	{
		protected override void Loaded()
		{
			var filePath = Path.Combine(DataDirectoryPath, "test.txt");
			//filePath = "C:\\text.txt";
			File.WriteAllText(filePath, AppDomain.CurrentDomain.FriendlyName);

			ClientApi.SampleApiMethod("Test argument.");

			ClientApi.SampleClientEvent += ClientApiOnSampleClientEvent;
		}

		private static void ClientApiOnSampleClientEvent(string argument)
		{
			Console.WriteLine($"Event on client triggered: {argument}. Executing assembly: {Assembly.GetExecutingAssembly()}");
		}
	}
}
