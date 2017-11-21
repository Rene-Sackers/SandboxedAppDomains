﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;

namespace CSharpSandbox.ClientScript
{
	public class SomeClientScript : ClientScriptDomain.ClientScript
	{
		protected override void Loaded()
		{
			var filePath = Path.Combine(DataDirectoryPath, "test.txt");
			//filePath = "C:\\text.txt";
			File.WriteAllText(filePath, AppDomain.CurrentDomain.FriendlyName);

			ClientApi.Test = "b";

			ClientApi.GameTick += ClientApiOnGameTick;
		}

		private void ClientApiOnGameTick()
		{
			Console.WriteLine("Game tick");
		}
	}
}
