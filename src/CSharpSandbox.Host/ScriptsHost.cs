using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpSandbox.Host
{
    public class ScriptsHost
    {
        private readonly List<ScriptInstance> _scriptInstances = new List<ScriptInstance>();

        public void Initialize()
        {
            var scriptsFolder = Path.GetFullPath("Scripts");

            var scriptInstances = Directory.GetDirectories(scriptsFolder).Select(GetScriptInstance);
            _scriptInstances.AddRange(scriptInstances);
        }

        private static ScriptInstance GetScriptInstance(string scriptFolder) => new ScriptInstance(scriptFolder);

        public void StartScripts() => _scriptInstances.Where(s => !s.IsRunning).ToList().ForEach(s => s.Start());
    }
}