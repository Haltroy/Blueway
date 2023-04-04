# Build

Blueway is an open-source application, which means users can build Blueway by themselves if they wanted to. This documentation helps to achieve that goal.

## Requirements

 - [.NET SDK 6.0](https://dotnet.microsoft.com/) or newer.
 - The source code of Blueway.
    - Clone this repository: `git clone https://github.com/haltroy/Blueway.git`
    - [Download ZIP](https://github.com/Haltroy/Blueway/archive/refs/heads/main.zip)


## Building

### Visual Studio

If you have Visual Studio 2022 or newer installed, the one simply can open the Blueway solution file and build Blueway from there instead by either running Blueway or right-clicking Blueway (NOTE: Not "Blueway.Standard", just "Blueway") and pressing "Build" option in Solution Explorer.

To build a release version of the Blueway, simply set configuration from "Debug" to "Release" on top bar next to where you launch Blueway.

Build files are located on `[Blueway project path]/bin/[Configuration]/` folder.

### .NET CLI

Either open a terminal emulator (CMD, Powershell, GNOME Terminal, Alacritty, Konsole, etc.) and navigate to the Blueway folder path or navigate to the path and open a terminal emulator. Either way, make sure a terminal emulator is visible and at the location of the project.

Then:

1. Execute `cd src/Blueway` (Unix family systems) or `src\Blueway` (Windows)
2. Execute `dotnet build` or `dotnet publish`
     - With no other options included, build files require .NET to be installed on machine to run it. .NET SDK already contains the runtime but if the build files are copied to another machine without .NET, it might not run. In order to run it on all machines, build a self-contained version of Blueway.
     - To build a release version of Blueway, add `-c Release` to command. `dotnet build -c Release`
     - To build a self-contained release of Blueway, add `--self-contained true -r [Your System Architecture]` to comand.
     - To build a single file version of Blueway, add `-r [Your System Architecture] -p:PublishSingleFile=true` to command.
     - To build a public release version of Blueway, add all above options and use `dotnet publish` instead. `dotnet publish -c Release --self-contained true -p:PublishSingleFile=true -r [Your System Architecture]`
3. Build files are located on `[Blueway project path]/bin/[Configuration]/` folder.

Here's a list of all System architectures:
 - Windows: `win-x86` (32-bit), `win-x64` (64-bit), `win-arm64` (ARM 64-bit)
 - Linux: `linux-x86` (32-bit), `linux-x64` (64-bit), `linux-musl-x64` (64-bit MUSL), `linux-arm` (ARM), `linux-arm64` (ARM 64-bit).
 - macOS: `osx-x64` (Intel/AMD), `osx.[Version]-arm64` (ARM Mac's with specific versions such as `11.0`, `12`, `13`)

## Tips & Tricks

### Trim Blueway

.NET lets users to trim their apps to save space in cost of API compatibility.

This has these effects on Blueway:
 - Since the external extensions are not included while building Blueway, the code that they use might get trimmed. This might make them incompatible.
 - Avalonia (the GUI engine of Blueway) might not work properly when trimmed.

To trim build Blueway, follow .NET CLI instructions but:
 - Use `dotnet publish` instead
 - Add `-p:PublishTrimmed true` option to the command.

To achieve the most space (Release configuration, self-contained, single file, trimmed): `dotnet publish -c Release --self-contained true -p:PublishSingleFile=true -r [Your System Architecture] -p:PublishTrimmed true`

### Disable external extension loading completely

To disable external extension loading completely to save startup time:

1. Open up `[Blueway project folder]\src\Blueway.Standard\Settings.cs` and change variable `GlobalEnableExtensionLoading` to `false`.
2. Save file and build Blueway.

### Disable extensions completely

To disable all extensions from loading completely to save startup time:

1. Open up `[Blueway project folder]\src\Blueway.Standard\Settings.cs` and change variable `GlobalEnableExtensions` to `false`.
2. Save file and build Blueway.

### Add your own extensions

To add custom extensions alongside Blueway, clone them to `src` folder (for project references) and add package/project references to Blueway.csproj file. Then simply follow the instructions in `[Blueway project folder]\src\Blueway.Standard\Settings.cs` in constructor `public Settings()`. Then build the app.

 - To add a package reference: `dotnet add package [Package Name]`.
 - To add a project reference: `dotnet add reference [Extension .csproj path]`

### Create custom extensions

See [EXTENSIONS.md](EXTENSIONS.md).