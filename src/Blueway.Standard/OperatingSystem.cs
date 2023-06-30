// TODO: change descriptions of these, i gave up.
namespace Blueway
{
    /// <summary>
    /// Holds informations about the operating systems we support.
    /// </summary>
    public static class OperatingSystems
    {
        /// <summary>
        /// Main class for the operating systems.
        /// </summary>
        public abstract class OperatingSystem
        {
            /// <summary>
            /// Name of the operating system, used for language system.
            /// </summary>
            public abstract string Name { get; }

            /// <summary>
            /// Command to pass to the package manager, must be automatic so no confirmation. Here's a list of parameters:
            /// <para />
            /// - %L% - Items in list <para />
            /// - %N% - Name of the application <para />
            /// - %A% - Author of the application <para />
            /// - %V% - Version of the application <para />
            /// </summary>
            public abstract string PackageManagerCommand { get; }

            /// <summary>
            /// <c>true</c> if the package manager allows multiple apps to be installed simultaneously (my-command app1 app2 app3 ...) or individually (my-command app1 && my-command app2 && my-command app3 ...).
            /// </summary>
            public abstract bool PackageManagerAllowsMultiple { get; }

            /// <summary>
            /// Arch to use when downloading the self-contained package URL. Use %a% for processor architecture (in .NET terms).
            /// </summary>
            public abstract string SelfContainedArch { get; }
        }

        /// <summary>
        /// For targeting all systems.
        /// </summary>
        public class AllSystems : OperatingSystem
        {
            public override string Name => "Any";

            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => throw new System.NotImplementedException();
        }

        /// <summary>
        /// For targeting Windows NT (7 SP1 or newer).
        /// </summary>
        public class Windows : AllSystems
        {
            public override string Name => "Windows";

            public override string PackageManagerCommand => ""; // TODO: execute batch script for regedit (cmd.exe /C [command-here])

            public override bool PackageManagerAllowsMultiple => false;

            public override string SelfContainedArch => "win-%a%";
        }

        /// <summary>
        /// Main class for the rest of world.
        /// </summary>
        public class Unix : AllSystems
        {
            public override string Name => "Unix";
            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => throw new System.NotImplementedException();
        }

        /// <summary>
        /// Main class for a main class for a main class for Apple stuff.
        /// </summary>
        public class BSD : Unix
        {
            public override string Name => "BSD";
            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => throw new System.NotImplementedException();
        }

        /// <summary>
        /// Main class for a main class for Apple stuff.
        /// </summary>
        public class FreeBSD : BSD
        {
            public override string Name => "FreeBSD";
            public override string PackageManagerCommand => ""; // TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => ""; // TODO
        }

        /// <summary>
        /// Main class for Apple stuff.
        /// </summary>
        public class Darwin : FreeBSD
        {
            public override string Name => "Apple";
            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => throw new System.NotImplementedException();
        }

        /// <summary>
        /// macOS, Macintosh, OSX
        /// </summary>
        public class MacOS : Darwin
        {
            public override string Name => "MacOS";
            public override string PackageManagerCommand => ""; //TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => "osx-%a%";
        }

        /// <summary>
        /// iPhone, iPad
        /// <para />
        /// NOTE: Apple has strict guidelines for NOT INSTALLING any stuff so we are not allowed to reinstall stuff but we can show user a list of apps and redirect to the store instead, it will be painful but that's either that or no support for iOS.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles

        public class iOS : Darwin
#pragma warning restore IDE1006 // Naming Styles
        {
            public override string Name => "iOS";
            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => throw new System.NotImplementedException();
        }

        /// <summary>
        /// Main class for distros like Debian, RedHat etc.
        /// <para />
        /// NOTE: All other distros that are based on these (such as Manjaro based on <see cref="Arch"/>) should use the distro they based on. Example: Garuda should use <see cref="Arch"/>. Mostly picked by command availability. Also, the distros that use source-based package management should use this class (<seealso cref="Linux"/>) instead (or fall back to here if the package managers do not work).
        /// </summary>
        public class Linux : Unix
        {
            public override string Name => "Linux";
            public override string PackageManagerCommand => "tar -xf %L% -C /";

            public override bool PackageManagerAllowsMultiple => false;

            public override string SelfContainedArch => "linux-%a%";
        }

        /// <summary>
        /// Uses APT. Has PPAs. Ubuntu, the majority of distros.
        /// </summary>
        public class Debian : Linux
        {
            public override string Name => "Debian";
            public override string PackageManagerCommand => "apt install -y %L%";

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Uses DNF or YUM. Has COPR. Fedora, EPEL, etc.
        /// </summary>
        public class RedHat : Linux
        {
            public override string Name => "RHEL";
            public override string PackageManagerCommand => "dnf install -y %L%";

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Distros with Flatpak support.
        /// </summary>
        public class Flatpak : Linux
        {
            public override string Name => "Flatpak";
            public override string PackageManagerCommand => "flatpak install -y %L%"; //TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Uses PACMAN, has AUR. Arch Linux, Artix, Manjaro, endeavour, Garuda etc.
        /// </summary>
        public class Arch : Linux
        {
            public override string Name => "Arch";
            public override string PackageManagerCommand => "pacman -S --noconfirm %L%";

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Femboy
        /// </summary>
        public class Gentoo : Linux
        {
            public override string Name => "Gentoo";
            public override string PackageManagerCommand => "emerge %L%"; // TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// NOTE: Same might happen to Android. Don't install anything. see <see cref="iOS"/> description.
        /// </summary>
        public class Android : Linux
        {
            public override string Name => "Android";

            public override string PackageManagerCommand => throw new System.NotImplementedException();

            public override bool PackageManagerAllowsMultiple => throw new System.NotImplementedException();

            public override string SelfContainedArch => "android-%a%";
        }

        /// <summary>
        /// Uses ZYPP, SUSE Enterprise Linux, openSUSE.
        /// </summary>
        public class SUSE : Linux
        {
            public override string Name => "SUSE";
            public override string PackageManagerCommand => "apt install -y %L%"; // TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Uses APK, might use MUSL.
        /// </summary>
        public class Alpine : Linux
        {
            public override string Name => "Alpine";
            public override string PackageManagerCommand => "apk add -y %L%";

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }

        /// <summary>
        /// Used for distros with Snapd.
        /// </summary>
        public class Snap : Linux
        {
            public override string Name => "Snap";
            public override string PackageManagerCommand => "snapd %L%"; // TODO

            public override bool PackageManagerAllowsMultiple => true;

            public override string SelfContainedArch => base.SelfContainedArch;
        }
    }
}