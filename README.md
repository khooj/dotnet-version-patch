# dotnet-version-patch
Dumb versioning tool that increment major and minor version in AssemblyInfo.cs. 

Supposed to be used as pre-build task.

## Usage example
### Installing tool

```dotnet tool install -g dotnet-version-patch```

### WebApplication1.csproj

We need to deny assembly info generation because tool will modify it for us.
Also we creating target that runs our tool when building in Release configuration.
Note that tool will increment minor version as we used '--minor' argument.
You can use '--major' and/or '--minor' so dotnet-version-patch will increment major and/or minor version accordingly.
```<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp2.2</TargetFramework>
    <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.AspNetCore.Razor.Design" Version="2.2.0" PrivateAssets="All" />
  </ItemGroup>
  <Target Name="VersionPatchTarget" BeforeTargets="PreBuildEvent">
    <Exec Command="dotnet version-patch --minor" Condition=" '$(Configuration)' == 'Release' " />
  </Target>
</Project>
```

### AssemblyInfo.cs
Create default AssemblyInfo.cs in projects' properties and modify AssemblyVersion and AssemblyFileVersion that both major and minor numbers the same.
```using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WebApplication1.Properties")]
[assembly: AssemblyTrademark("")]
// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ae2a4183-9446-493c-aac2-d2817b89411b")]
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1")]
```
Tool also supports autoincrementing build and revision by VisualStudio.
```
[assembly: AssemblyVersion("1.1.*")]
[assembly: AssemblyFileVersion("1.1")]
```
After that before every Release build tool will increment minor version in AssemblyInfo.cs and that number will appear in resulting assembly.
