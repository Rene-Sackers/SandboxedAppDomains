namespace CSharpSandbox.ClientSharedApi
{
	public interface IClientApi
	{
		event EventDelegates.GameTickHandler GameTick;
	}
}