using System;
using System.IO;
using System.Reflection;

namespace CSharpSandbox.ClientScript
{
	public class SampleClientScript : ClientSharedApi.ClientScript
	{
		protected override void Loaded()
		{
			var filePath = Path.Combine(DataDirectoryPath, "test.txt"); // Should be allowed
			//filePath = "C:\\text.txt"; // Will cause permission error
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
