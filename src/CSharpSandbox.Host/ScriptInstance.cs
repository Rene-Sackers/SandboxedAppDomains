using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml.Schema;
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

            var dllFiles = GetDllFiles();
            var dllFile = dllFiles.First();

            var permissionSet = new PermissionSet(PermissionState.Unrestricted);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.AllFlags));

            var fullTrustAssembly = typeof(ScriptInstance).Assembly.Evidence.GetHostEvidence<StrongName>();
            var fullTrustAssembly2 = typeof(Loader).Assembly.Evidence.GetHostEvidence<StrongName>();

            var scriptDomainSetup = new AppDomainSetup
            {
                ApplicationBase = _scriptFolderPath,
                ApplicationTrust = new ApplicationTrust(permissionSet, new [] { fullTrustAssembly, fullTrustAssembly2 })
            };

            //var clientScriptAppDomain = AppDomainContext.Create(scriptDomainSetup);
            var clientScriptAppDomain = AppDomainContext.Create();
            var loadedAssembly = clientScriptAppDomain.LoadAssembly(LoadMethod.LoadBits, dllFile);

            var remoteScript = Remote<Loader>.CreateProxy(clientScriptAppDomain.Domain);
            remoteScript.RemoteObject.LoadClientScripts();
        }

        private bool CheckValidType(Type type) => type.IsAssignableFrom(typeof(ClientScript)) && !type.IsAbstract && !type.IsInterface;

        private IEnumerable<string> GetDllFiles() => Directory.GetFiles(_scriptFolderPath, "*.dll");
    }
}