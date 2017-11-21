# C# Sandboxed App Domains example

This is an example on how to create secure(1) sandboxed app domains in order to execute untrusted code safely(1)

(1): The "secureness"/"safety" of this code is entirely debatable, and not guaranteed. Don't hold me liable.

## Granted permissions

The following permissions have been granted to the child domains:

* ReflectionPermissionFlag.MemberAccess
* SecurityPermissionFlag.Execution
* SecurityPermissionFlag.SerializationFormatter
* FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, in the directory of the client script
* FileIOPermissionAccess.Read | FileIOPermissionAccess.Write | FileIOPermissionAccess.Append | FileIOPermissionAccess.PathDiscovery, in the subdirectory "Data" from the client script's .dll

More explanation can be found under the `CreatePermissionSet()` method in [ScriptInstance.cs](src/CSharpSandbox.Host/ScriptInstance.cs)

## AppDomainToolkit

This project uses [@jduv](https://github.com/jduv)'s [AppDomainToolkit](https://github.com/jduv/AppDomainToolkit). Which makes it significantly easier to set up app domains like this.

Due to an issue discovered in the code, a custom compiled version of this library has been included (with permission) until the NuGet package gets updated. See [issue #25](https://github.com/jduv/AppDomainToolkit/issues/25).

## .pfx password

In case you need it:

```
CSharpSandbox
```