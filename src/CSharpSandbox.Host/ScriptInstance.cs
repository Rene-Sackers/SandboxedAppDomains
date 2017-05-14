using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using AppDomainToolkit;
using CSharpSandbox.SharedApi;

namespace CSharpSandbox.Host
{
    public class ScriptInstance
    {
        private readonly string _scriptFolderPath;
        
        public bool IsRunning { get; private set; }

        public ScriptInstance(string scriptFolderPath)
        {
            _scriptFolderPath = scriptFolderPath;
        }

        public void Start()
        {
            IsRunning = true;

            var dllFiles = GetDllFiles().ToList();

            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, _scriptFolderPath));

            var allowedStrongNames = GetLoadedAssembliesStrongNames(permissionSet);

            //var extensionStrongNames = GetStrongNamesForFiles(dllFiles);
            //allowedStrongNames.AddRange(extensionStrongNames);

            var scriptDomainSetup = new AppDomainSetup
            {
                ApplicationTrust = new ApplicationTrust(permissionSet, allowedStrongNames)
            };

            var clientScriptAppDomain = AppDomainContext.Create(scriptDomainSetup);

            foreach (var dllFile in dllFiles)
                clientScriptAppDomain.LoadAssembly(LoadMethod.LoadFile, dllFile);

            var remoteScript = Remote<Loader>.CreateProxy(clientScriptAppDomain.Domain);
            remoteScript.RemoteObject.LoadClientScripts(_scriptFolderPath);
        }

        private static IEnumerable<StrongName> GetStrongNamesForFiles(IEnumerable<string> dllFiles)
        {
            return dllFiles
                .Select(Assembly.ReflectionOnlyLoadFrom)
                .Select(assembly => assembly?.Evidence.GetHostEvidence<StrongName>())
                .Where(dllFileStrongName => dllFileStrongName != null);
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

        private IEnumerable<string> GetDllFiles() => Directory.GetFiles(_scriptFolderPath, "*.dll");
        
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