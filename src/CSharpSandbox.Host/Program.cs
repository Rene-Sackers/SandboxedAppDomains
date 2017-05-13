using System;

namespace CSharpSandbox.Host
{
    public class Program
    {
        public static void Main()
        {
            var scriptsHost = new ScriptsHost();
            scriptsHost.Initialize();
            scriptsHost.StartScripts();

            while (true)
                Console.ReadKey();
        }
    }
}
