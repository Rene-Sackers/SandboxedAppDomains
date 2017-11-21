namespace CSharpSandbox.ClientSharedApi
{
	public interface IClientApi
	{
		event EventDelegates.SampleClientEventHandler SampleClientEvent;

		void SampleApiMethod(string argument);
	}
}