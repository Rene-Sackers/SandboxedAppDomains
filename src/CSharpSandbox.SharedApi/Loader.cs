using System;
using System.Linq;

namespace CSharpSandbox.SharedApi
{
    internal class Loader : MarshalByRefObject
    {
        public void LoadClientScripts(string baseDirectory)
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
                clientScript.BaseDirectory = baseDirectory;
                clientScript.CallLoaded();
            }
        }
    }
}