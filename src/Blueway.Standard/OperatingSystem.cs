// TODO: change descriptions of these, i gave up.
namespace ProjectKolme
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
        }

        /// <summary>
        /// For targeting all systems.
        /// </summary>
        public class AllSystems : OperatingSystem
        {
            public override string Name => "Any";
        }

        /// <summary>
        /// For targeting Windows NT (7 SP1 or newer).
        /// </summary>
        public class Windows : AllSystems
        {
            public override string Name => "Windows";
        }

        /// <summary>
        /// Main class for the rest of world.
        /// </summary>
        public class Unix : AllSystems
        {
            public override string Name => "Unix";
        }

        /// <summary>
        /// Main class for a main class for a main class for Apple stuff.
        /// </summary>
        public class BSD : Unix
        {
            public override string Name => "BSD";
        }

        /// <summary>
        /// Main class for a main class for Apple stuff.
        /// </summary>
        public class FreeBSD : BSD
        {
            public override string Name => "FreeBSD";
        }

        /// <summary>
        /// Main class for Apple stuff.
        /// </summary>
        public class Darwin : FreeBSD
        {
            public override string Name => "Apple";
        }

        /// <summary>
        /// macOS, Macintosh, OSX
        /// </summary>
        public class MacOS : Darwin
        {
            public override string Name => "MacOS";
        }

        /// <summary>
        /// iPhone, iPad
        /// <para />
        /// NOTE: Apple has strict guidelines for NOT INSTALLING any stuff so we are not allowed to reinstall stuff but we can show user a list of apps and redirect to the store istead, it will be painful but that's either that or no support for iOS.
        /// </summary>
        public class iOS : Darwin
        {
            public override string Name => "iOS";
        }

        /// <summary>
        /// Main class for distros like Debian, RedHat etc.
        /// <para />
        /// NOTE: All other distros that are based on these (such as Manjaro based on <see cref="Arch"/>) should use the distro they based on. Example: Garuda should use <see cref="Arch"/>. Mostly picked by command availability. Also, the distros that use source-based package management should use this class (<seealso cref="Linux"/>) instead (or fallback to here if the package managers did not work).
        /// </summary>
        public class Linux : Unix
        {
            public override string Name => "Linux";
        }

        /// <summary>
        /// Uses APT. Has PPAs. Ubuntu, the majority of distros.
        /// </summary>
        public class Debian : Linux
        {
            public override string Name => "Debian";
        }

        /// <summary>
        /// Uses DNF or YUM. Has COPR. Fedora, EPEL, etc.
        /// </summary>
        public class RedHat : Linux
        {
            public override string Name => "RHEL";
        }

        /// <summary>
        /// Same as <see cref="RedHat"/> but with flatpak support. DNF, COPR, Flatpak.
        /// </summary>
        public class Fedora : RedHat
        {
            public override string Name => "Fedora";
        }

        /// <summary>
        /// Uses PACMAN, has AUR. Arch Linux, Artix, Manjaro, endevaour, Garuda etc.
        /// </summary>
        public class Arch : Linux
        {
            public override string Name => "Arch";
        }

        /// <summary>
        /// Femboy
        /// </summary>
        public class Gentoo : Linux
        {
            public override string Name => "Gentoo";
        }

        /// <summary>
        /// NOTE: Same might happen to Android. Don't install anything. see <see cref="iOS"/> description.
        /// </summary>
        public class Android : Linux
        {
            public override string Name => "Android";
        }

        /// <summary>
        /// Uses ZYPP, SUSE Enterprise Linux, openSUSE.
        /// </summary>
        public class SUSE : Linux
        {
            public override string Name => "SUSE";
        }

        /// <summary>
        /// Uses APK, might use MUSL.
        /// </summary>
        public class Alpine : Linux
        {
            public override string Name => "Alpine";
        }

        /// <summary>
        /// Inherits Debian but contains Snap by default, we can use snapd too. APT, PPAs and snapd.
        /// </summary>
        public class Ubuntu : Debian
        {
            public override string Name => "Ubuntu";
        }
    }
}