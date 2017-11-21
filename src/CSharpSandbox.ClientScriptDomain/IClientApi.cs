namespace CSharpSandbox.ClientScriptDomain
{
	public interface IClientApi
	{
		event EventDelegates.SampleClientEventHandler SampleClientEvent;

		void SampleApiMethod(string argument);
	}
}