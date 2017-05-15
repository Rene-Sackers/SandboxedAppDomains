using System;
using System.Diagnostics;
using System.IO;

namespace CSharpSandbox.ClientScript
{
    public class SomeClientScript : SharedApi.ClientScript
    {
        protected override void Loaded()
        {
            var filePath = Path.Combine(BaseDirectory, "test.txt");
            File.WriteAllText(filePath, AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
