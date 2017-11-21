using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using AppDomainToolkit;
using CSharpSandbox.ClientSharedApi;

namespace CSharpSandbox.Host
{
	public class ScriptInstance
	{
		private readonly string _scriptDirectoryPath;
		private readonly ClientApi _clientApi;
		private readonly string _scriptDataDirectoryPath;

		public bool IsRunning { get; private set; }

		public ScriptInstance(string scriptDirectoryPath, ClientApi clientApi)
		{
			_scriptDirectoryPath = scriptDirectoryPath;
			_clientApi = clientApi;
			_scriptDataDirectoryPath = Path.Combine(_scriptDirectoryPath, "Data");

			Directory.CreateDirectory(_scriptDataDirectoryPath);
		}

		public void Start()
		{
			IsRunning = true;

			var clientScriptAppDomain = CreateScriptAppDomain();

			var remoteScript = Remote<Loader>.CreateProxy(clientScriptAppDomain.Domain);

			try
			{
				remoteScript.RemoteObject.LoadClientScripts(_scriptDataDirectoryPath, _clientApi);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to initialize script {_scriptDirectoryPath}:\r\n" + e.Message);
			}
		}

		private AppDomainContext<AssemblyTargetLoader, PathBasedAssemblyResolver> CreateScriptAppDomain()
		{
			var permissionSet = CreatePermissionSet();
			var allowedStrongNames = GetLoadedAssembliesStrongNames(permissionSet);

			var scriptDomainSetup = new AppDomainSetup
			{
				ApplicationTrust = new ApplicationTrust(permissionSet, allowedStrongNames),
			};

			var clientScriptAppDomain = AppDomainContext.Create(scriptDomainSetup);
			clientScriptAppDomain.AssemblyImporter.AddProbePath(_scriptDirectoryPath);
			clientScriptAppDomain.RemoteResolver.AddProbePath(_scriptDirectoryPath);

			foreach (var dllFile in GetDllFiles())
				clientScriptAppDomain.LoadAssembly(LoadMethod.LoadFrom, dllFile);

			return clientScriptAppDomain;
		}

		private PermissionSet CreatePermissionSet()
		{
			var permissionSet = new PermissionSet(PermissionState.None);

			permissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess)); // Needed for AppDomainToolkit loader. AppDomainContext.cs, line 106, RemoteAction.Invoke(...)

			permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution)); // Needed to load and execute the assembly at all
			permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter)); // Needed when accessing ClientApi on the provided IClientApi interface from the plugin script. Why? Not sure.

			permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, _scriptDirectoryPath));
			permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write | FileIOPermissionAccess.Append | FileIOPermissionAccess.PathDiscovery, _scriptDataDirectoryPath));

			return permissionSet;
		}

		private static IEnumerable<StrongName> GetLoadedAssembliesStrongNames(PermissionSet permissionSet)
		{
			var allowedLibraries = new List<string>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var path = GetAssemblyLocation(assembly);
				if (!string.IsNullOrWhiteSpace(path))
				{
					allowedLibraries.Add(path);
				}

				var tmpPath = assembly.ManifestModule.FullyQualifiedName;
				if (path != tmpPath && File.Exists(tmpPath))
					allowedLibraries.Add(tmpPath);

				if (assembly.GlobalAssemblyCache && assembly.FullName.StartsWith("System"))
				{
					allowedLibraries.Add(GetSystemPaths(assembly));
				}

				var strongName = assembly.Evidence.GetHostEvidence<StrongName>();
				if (strongName != null)
					yield return strongName;
			}

			permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, allowedLibraries.ToArray()));
		}

		private IEnumerable<string> GetDllFiles()
		{
			var loadedAssembliesFullNames = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName).ToList();

			foreach (var dllFile in Directory.GetFiles(_scriptDirectoryPath, "*.dll"))
			{
				var dllFileFullName = Assembly.ReflectionOnlyLoadFrom(dllFile)?.FullName;
				if (string.IsNullOrWhiteSpace(dllFileFullName)) continue;
				if (loadedAssembliesFullNames.Contains(dllFileFullName)) continue;

				yield return dllFile;
			}
		}

		private static string GetAssemblyLocation(Assembly assembly)
		{
			if (assembly.IsDynamic)
				return null;

			var codeBase = assembly.CodeBase;
			var uri = new UriBuilder(codeBase);

			var path = Uri.UnescapeDataString(uri.Path);
			path = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, Path.GetFileName(assembly.Location) ?? string.Empty);

			return path;
		}

		private static string GetSystemPaths(Assembly assembly)
		{
			var path = $@"C:\Windows\Microsoft.NET\Framework\{assembly.ImageRuntimeVersion}\{assembly.ManifestModule.Name}";
			return path;
		}
	}
}