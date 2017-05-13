using System;
using System.Linq;

namespace CSharpSandbox.SharedApi
{
    public class Loader : MarshalByRefObject
    {
        public void LoadClientScripts()
        {
            var allTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .ToList();

            var scriptTypes = allTypes.Where(type => typeof(ClientScript).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToList();

            foreach (var scriptType in scriptTypes)
                Activator.CreateInstance(scriptType);
        }
    }
}