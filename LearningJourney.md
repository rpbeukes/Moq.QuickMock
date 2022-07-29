#  Visual Studio Extensions
This is my first attempt at a Visual Studio Extension.

Let the fun begin :)

## Fantastic resource:
Google: [mads kristensen extensions](https://www.google.com/search?q=mads+kristensen+extensions&rlz=1C1RXQR_en-GBAU944AU944&oq=mads+kristensen+extensions&aqs=chrome..69i57.264j0j7&sourceid=chrome&ie=UTF-8)

[Visual Studio Extensibility Cookbook](https://www.vsixcookbook.com/getting-started/your-first-extension.html)

## Visual Studio Extensions Marketplace
[Marketplace](https://marketplace.visualstudio.com/)

## Tools
- [Extensibility Essentials 2022](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityEssentials2022)
- [Known Monikers Explorer 2022](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.KnownMonikersExplorer2022)
- [Code Analysis With Roslyn's Syntax Trees](https://www.codingzee.com/2021/04/c-code-analysis-with-roslyn-syntax-trees.html)
  -  VS `Syntax Visualizer`: 
      
      > If you've already installed the .NET Compiler Platform SDK for your VS, use the following path to access this window: View -> Other Windows -> Syntax Visualizer.
  - VS `DGML editor`
     >  To install it, open the Visual Studio Installer. Locate the required VS installation instance, and click More -> Modify. Then switch to the following tab: Individual Component -> Code tools -> DGML editor.


## Github
- [mads](https://github.com/madskristensen)
- [RefactoringEssentials](https://github.com/icsharpcode/RefactoringEssentials) - thanx to icsharpcode community.
- [Mocking.Helpers](https://github.com/MrLuje/Mocking.Helpers) - thanx to MrLuje.
- [moq.autocomplete](https://github.com/Litee/moq.autocomplete) - thanx to Litee (Andrey Lipatkin)

## Github Actions - Build Pipeline
- [microsoft/setup-msbuild](https://github.com/microsoft/setup-msbuild)
- [setup-nuget](https://github.com/NuGet/setup-nuget)
- [actions/upload-artifact](https://github.com/actions/upload-artifact)
- https://stackoverflow.com/questions/69274658/github-action-nuget-restore-net-4-7-2
- https://cezarypiatek.github.io/post/develop-vsextension-with-github-actions/
- [VSIX Version](https://github.com/marketplace/actions/vsix-version) 
- [Extension versioning](https://cezarypiatek.github.io/post/develop-vsextension-with-github-actions/#how-to-set-the-version-for-vsix-file)

## Various resources:
- https://roslynquoter.azurewebsites.net/
- https://stackoverflow.com/questions/43804765/roslyn-get-identifiername-in-objectcreationexpressionsyntax
- https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-transformation
- https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.objectcreationexpressionsyntax?view=roslyn-dotnet-4.2.0
- https://www.filipekberg.se/2011/10/20/using-roslyn-to-parse-c-code-files/
- https://blog.emirosmanoski.mk/2020-11-02-Roslyn-Roslyn-Analyzer-Part2/

---
## Troubleshoot

### VS Template build issue
After creating a new VS solution via template `Code Refactor (.NET Standard)`, it could not build.

Fix it by navigating to `C:\Users\{YourUser}\.nuget\packages\microsoft.vssdk.buildtools` and replaced the latest verion number found there.
In my case: `15.1.192` => `17.1.4058`.

Open the solution folder and replaced all `15.1.192` occurrences with `17.1.4058`.

Error:
```
Build started...
1>------ Build started: Project: Moq.QuickMock, Configuration: Debug Any CPU ------
1>Skipping analyzers to speed up the build. You can execute 'Build' or 'Rebuild' command to run analyzers.
1>Moq.QuickMock -> C:\Ru\Moq.QuickMock\Moq.QuickMock\Moq.QuickMock\bin\Debug\netstandard2.0\Moq.QuickMock.dll
2>------ Build started: Project: Moq.QuickMock.Vsix, Configuration: Debug Any CPU ------
2>C:\Users\BlahBlah\.nuget\packages\microsoft.vssdk.buildtools\15.1.192\tools\VSSDK\Microsoft.VsSDK.targets(84,5): error MSB4062: The "CompareBuildTaskVersion" task could not be loaded from the assembly C:\Users\BlahBlah\.nuget\packages\microsoft.vssdk.buildtools\15.1.192\tools\VSSDK\Microsoft.VisualStudio.Sdk.BuildTasks.15.0.dll. Could not load file or assembly 'file:///C:\Users\BlahBlah\.nuget\packages\microsoft.vssdk.buildtools\15.1.192\tools\VSSDK\Microsoft.VisualStudio.Sdk.BuildTasks.15.0.dll' or one of its dependencies. An attempt was made to load a program with an incorrect format. Confirm that the <UsingTask> declaration is correct, that the assembly and all its dependencies are available, and that the task contains a public class that implements Microsoft.Build.Framework.ITask.
2>Done building project "Moq.QuickMock.Vsix.csproj" -- FAILED.
CodeRush canceled the build because project Moq.QuickMock\Moq.QuickMock.Vsix\Moq.QuickMock.Vsix.csproj failed to build. You can disable this behavior in the CodeRush options on the IDE\Build options page.
Build has been canceled.
```

---

### Make extension a VS2022 one

update `source.extension.vsixmanifest`:

```
  ...
  <Installation>
    <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0,18.0)">
      <ProductArchitecture>amd64</ProductArchitecture>
    </InstallationTarget>
  </Installation>
  ...
```

---

### VS Extension debugger stopped working (does not hit breakpoints)

Found very good help from Microsoft's [Troubleshoot Breakpoints in the Visual Studio Debugger](https://docs.microsoft.com/en-us/visualstudio/debugger/troubleshooting-breakpoints?view=vs-2022).

> Go to the Modules window (Debug > Windows > Modules) and check whether your module is loaded.

- In the searchbox, I typed `moq.`, and I saw that `Symbol Status` indicated: `file not found`.
- Then `Right-Clicked` on my file and choose `Open File Location`.
- Then dig around in there, and deleted my whole folder 
  eg: `C:\Users\{YourUser}}\AppData\Local\Microsoft\VisualStudio\17.0_ad6c95ffRoslyn`
- It seemed like a new VS Instance, with the correct extensions and now it knows how to load the symbols again. 

More options [here](https://github.com/dotnet/roslyn-sdk/issues/889#issuecomment-929608681) on github to solve the issue a different way.
I left my [solution](https://github.com/dotnet/roslyn-sdk/issues/889#issuecomment-1146767006) on that thread too.


Investigation tools: 
- [ms sysinternals](https://docs.microsoft.com/en-us/sysinternals/)
-  [Microsoft Child Process Debugging Power Tool 2022](https://marketplace.visualstudio.com/items?itemName=vsdbgplat.MicrosoftChildProcessDebuggingPowerTool2022)

## Video to Gif conversion
[cloudconvert](https://cloudconvert.com/mp4-to-gif)
