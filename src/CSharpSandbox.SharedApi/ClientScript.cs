namespace CSharpSandbox.SharedApi
{
    public abstract class ClientScript
    {
        public string BaseDirectory { get; internal set; }
        
        public abstract void Loaded();
    }
}