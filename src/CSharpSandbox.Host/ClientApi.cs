﻿using System;
using System.Reflection;
using CSharpSandbox.ClientScriptDomain;

#pragma warning disable 67

namespace CSharpSandbox.Host
{
	public class ClientApi : MarshalByRefObject, IClientApi
	{
		private readonly Type _selfType;

		public event EventDelegates.SampleClientEventHandler SampleClientEvent;

		public ClientApi()
		{
			_selfType = GetType();
		}

		public void SampleApiMethod(string argument)
		{
			Console.WriteLine($"Client API method called. Argument: {argument}. Executing assembly: {Assembly.GetExecutingAssembly()}");
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
