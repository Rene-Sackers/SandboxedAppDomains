using System;
using System.Reflection;

#pragma warning disable 67

namespace CSharpSandbox.ClientSharedApi
{
	public class ClientApi : MarshalByRefObject
	{
		private readonly Type _selfType;

		public delegate void GameTickHandler();

		public event GameTickHandler GameTick;

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
