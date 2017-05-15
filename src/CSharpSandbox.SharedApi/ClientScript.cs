namespace CSharpSandbox.SharedApi
{
    public abstract class ClientScript
    {
        public string BaseDirectory { get; internal set; }

        internal void CallLoaded()
        {
            Loaded();
        }
        
        protected abstract void Loaded();
    }
}