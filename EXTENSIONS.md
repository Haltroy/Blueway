# Blueway Extensions

Blueway allows users to develop their own extensions and also lets users to backup the extensions alongside with the app itself (just like themes) without any hesitation.

Since Blueway is written in C#, the extensions should also be a .NET library (written in either C# or Visual Basic .NET). C# is similar to C and C++ which means some codes might be easy to convert, C# also supports [wrapping](https://learn.microsoft.com/en-us/cpp/dotnet/how-to-wrap-native-class-for-use-by-csharp?view=msvc-170).

## Requirements

For themes:
 1. Blueway
 2. [Fostrian Viewer]() or any HEX editor for manipulating [Fostrian]() files.

For custom backup actions:
 - .NET SDK (latest long-term recommended).
 - a text editor or an Integrated Development Environment (IDE) that supports C# or Visual Basic .NET (such as Visual Studio, Visual Studio Code, etc.)

## Limitations

Blueway uses .NET's self-contained features to embed the runtime with the app, however some users that built the app from source can disable it as well as build it with trimming to save space.

In default, Blueway is self-contained .NET app which comes with these limitations:
 - Don't use any one of these APIs as they will throw an exception: `Assembly.CodeBase` , `Assembly.EscapedCodeBase`, `Assembly.GetFile`, `Assembly.GetFiles`
 - These APIs return a null, invalid or an empty value, usage is not recommended and mostly not needed: `Assembly.Location`, `AssemblyName.CodeBase`, `AssemblyName.EscapedCodeBase`, `Module.FullyQualifiedName`, `Marshal.GetHINSTANCE`, `Module.Name`
 - Use "Embedded Resources" for files that you need.

For a default Blueway installation, the above is the only limitations. However, if you are developing an extension and also going to build Blueway by your own, here's some limitations about trimming and AoT compiling.

**Trimming:**
 - Avalonia (the GUI engine of Blueway) might not work properly when trimmed.
 - Blueway uses reflection for extensions, which is out of the scope of .NET's trimming feature and might trim a code that was going to be used by one of the extensions. Consider disabling external extensions and loading the extensions that you need from code.


## Development

### Themes

There are 2 ways to create new themes for Blueway.

#### From Blueway Settings

1. Launch Blueway and open the settings menu.
2. Go to "Customization" tab.
3. Use the options to create the theme.
4. Press `Save` button.
5. Your theme should be in `[Your OS user folder\.blueway\themes]` folder.

#### From Fostrian

Blueway themes are just Fostrian files. To create a theme without using Blueway, simply download [this template]() and edit it.

### Custom Backup Actions