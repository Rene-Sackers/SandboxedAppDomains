using System;
using System.IO;

namespace CSharpSandbox.ClientScript
{
    public class SomeClientScript : SharedApi.ClientScript
    {
        public override void Loaded()
        {
            var filePath = Path.Combine(BaseDirectory, "test.txt");
            File.WriteAllText(filePath, AppDomain.CurrentDomain.FriendlyName);
            //Process.Start(filePath);
        }
    }
}
