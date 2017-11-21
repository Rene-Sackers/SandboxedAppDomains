using System;
using System.Reflection;
using CSharpSandbox.ClientSharedApi;

#pragma warning disable 67

namespace CSharpSandbox.Host
{
	public class ClientApi : MarshalByRefObject, IClientApi
	{
		private readonly Type _selfType;

		public event EventDelegates.GameTickHandler GameTick;

		public ClientApi()
		{
			_selfType = GetType();
		}
		
		internal void RaiseEvent(string eventName, params object[] arguments)
		{
			var eventDelegate = (MulticastDelegate)_selfType.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
			if (eventDelegate == null) return;

			foreach (var handler in eventDelegate.GetInvocationList())
				handler.Method.Invoke(handler.Target, arguments);
		}
	}
}
