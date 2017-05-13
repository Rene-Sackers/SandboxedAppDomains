using System.IO;

namespace CSharpSandbox.ClientScript
{
    public class SomeClientScript : SharedApi.ClientScript
    {
        public SomeClientScript()
        {
            File.WriteAllText("test.txt", "initialized");
        }
    }
}
