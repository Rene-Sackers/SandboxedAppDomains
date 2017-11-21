using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpSandbox.ClientSharedApi;

namespace CSharpSandbox.Host
{
	public class ScriptsHost
	{
		private readonly ClientApi _clientApi;
		private readonly List<ScriptInstance> _scriptInstances = new List<ScriptInstance>();

		public ScriptsHost(ClientApi clientApi)
		{
			_clientApi = clientApi;
		}

		public void Initialize()
		{
			var scriptsFolder = Path.GetFullPath("Scripts");

			var scriptInstances = Directory.GetDirectories(scriptsFolder).Select(GetScriptInstance);
			_scriptInstances.AddRange(scriptInstances);
		}

		private ScriptInstance GetScriptInstance(string scriptFolder) => new ScriptInstance(scriptFolder, _clientApi);

		public void StartScripts() => _scriptInstances.Where(s => !s.IsRunning).ToList().ForEach(s => s.Start());
	}
}