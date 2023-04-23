using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibFoster;

namespace Blueway
{
    public class Settings
    {
        public static bool GlobalEnableExtensions => true;
        public static bool GlobalEnableExtensionLoading => true;

        #region Paths

        private string UserFiles => Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".blueway");
        private string UserSettingsFile => Path.Combine(UserFiles, "settings.fff");
        private string AppSettingsFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.fff");
        private string UserHistoryFile => Path.Combine(UserFiles, "history.fff");
        private string AppHistoryFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.fff");
        private string UserThemes => Path.Combine(UserFiles, "themes");
        private string AppThemes => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes");
        private string UserSourcesFolder => Path.Combine(UserFiles, "sources");
        private string AppSourcesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sources");
        private string UserExts => Path.Combine(UserFiles, "ext");
        private string AppExts => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ext");

        #endregion Paths

        #region Settings

        public bool EnableExtensions { get; set; } = true;
        public bool CheckInternetConnection { get; set; } = true;
        public bool AutoBackupShowNotifications { get; set; } = true;
        public bool CopySettingsToBackup { get; set; } = true;
        public bool ShowNotificationOnUpdate { get; set; } = true;
        public bool AutoUpdate { get; set; } = true;
        public bool StartOnOS { get; set; } = true;
        public bool StartMinimized { get; set; } = true;
        public int ThreadCount { get; set; } = 2;
        public Theme CurrentTheme { get; set; } = DefaultThemes.Light;
        public BackupActionType[] BackupActionTypes { get; set; }
        public List<Theme> Themes { get; set; } = new List<Theme>() { DefaultThemes.Light, DefaultThemes.Dark, DefaultThemes.Breath, DefaultThemes.Breeze, DefaultThemes.Backupster };

        public List<BackupHistoryItem> History { get; set; } = new List<BackupHistoryItem>();

        public BackupHistoryItem GetLatestOrEmptyBackup()
        {
            if (History.Count > 0)
            {
                History = History.OrderBy(it => it.Date).ToList();
                return History.Last();
            }
            else
            {
                return new BackupHistoryItem("New Backup" /* TODO: Add translation here */, new BackupSchema(new List<BackupAction>()), DateTime.Now, BackupStatus.Planned, DateTime.Now, new TimeSpan(0), Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }

        }

        #endregion Settings

        public Settings()
        {
            if (!Directory.Exists(UserFiles))
            {
                Directory.CreateDirectory(UserFiles);
            }
            BackupActionTypes = new BackupActionType[] { new BackupActions.BackupDirectoryActionType() }; // TODO: add defaults
        }

        public Settings AutoLoadConfig()
        {
            if (File.Exists(UserSettingsFile))
            {
                ParseConfig(UserSettingsFile);
                return this;
            }
            if (File.Exists(AppSettingsFile))
            {
                ParseConfig(AppSettingsFile);
            }
            return this;
        }

        public Settings Save(string saveTo = null)
        {
            var root = Fostrian.GenerateRootNode(false);
            root.Encoding = System.Text.Encoding.Unicode;
            root.StartByte = 0x02;
            root.EndByte = 0x03;
            root.Add(new Fostrian.FostrianNode(EnableExtensions, "enable-extensions"));
            root.Add(new Fostrian.FostrianNode(EnableExtensions, "enable-extensions"));
            root.Add(new Fostrian.FostrianNode(CheckInternetConnection, "check-internet"));
            root.Add(new Fostrian.FostrianNode(AutoBackupShowNotifications, "auto-backup-notif"));
            root.Add(new Fostrian.FostrianNode(CopySettingsToBackup, "copy-settings"));
            root.Add(new Fostrian.FostrianNode(ShowNotificationOnUpdate, "update-notif"));
            root.Add(new Fostrian.FostrianNode(AutoUpdate, "update-auto"));
            root.Add(new Fostrian.FostrianNode(StartOnOS, "os-start"));
            root.Add(new Fostrian.FostrianNode(StartMinimized, "start-minimized"));
            root.Add(new Fostrian.FostrianNode(ThreadCount, "threads"));
            root.Add(new Fostrian.FostrianNode(CurrentTheme.Name, "theme"));
            Fostrian.Recreate(root, saveTo ?? UserSettingsFile);
            return this;
        }

        private void ParseConfig(string configFile)
        {
            // TODO: Load App Sources here

            // Load Themes
            List<string> themes = new List<string>();

            if (Directory.Exists(UserThemes))
            {
                themes.AddRange(Directory.GetFiles(UserThemes, "*.fff", SearchOption.AllDirectories));
            }

            if (Directory.Exists(AppThemes))
            {
                themes.AddRange(Directory.GetFiles(AppThemes, "*.fff", SearchOption.AllDirectories));
            }

            for (int i = 0; i < themes.Count; i++)
            {
                try
                {
                    Themes.Add(new Theme(themes[i]));
                }
                catch (Exception) { continue; }
            }
            themes = null;

            // Load Settings
            if (new FileInfo(configFile).Length <= 0)
            {
                return;
            }
            var root = Fostrian.Parse(configFile);
            for (int i = 0; i < root.Size; i++)
            {
                var node = root[i];
                if (node.Type == Fostrian.NodeType.FFF || string.IsNullOrWhiteSpace(node.Name)) { continue; }
                try
                {
                    switch (node.Name.ToLowerInvariant())
                    {
                        case "enable-extensions":
                            EnableExtensions = node.DataAsBoolean;
                            break;

                        case "check-internet":
                            CheckInternetConnection = node.DataAsBoolean;
                            break;

                        case "auto-backup-notif":
                            AutoBackupShowNotifications = node.DataAsBoolean;
                            break;

                        case "copy-settings":
                            CopySettingsToBackup = node.DataAsBoolean;
                            break;

                        case "update-notif":
                            ShowNotificationOnUpdate = node.DataAsBoolean;
                            break;

                        case "update-auto":
                            AutoUpdate = node.DataAsBoolean;
                            break;

                        case "os-start":
                            StartOnOS = node.DataAsBoolean;
                            break;

                        case "start-minimized":
                            StartMinimized = node.DataAsBoolean;
                            break;

                        case "threads":
                            ThreadCount = node.DataAsInt32;
                            break;

                        case "theme":
                            CurrentTheme = Themes.FindAll(it => string.Equals(it.Name, node.DataAsString)) is List<Theme> list && list.Count > 0 ? list[0] : DefaultThemes.Light;
                            break;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public Settings AutoLoadExtensions()
        {
            if (GlobalEnableExtensionLoading && GlobalEnableExtensions && EnableExtensions)
            {
                List<string> exts = new List<string>();
                List<BackupActionType> current = new List<BackupActionType>();
                current.AddRange(BackupActionTypes);

                if (Directory.Exists(UserExts))
                {
                    exts.AddRange(Directory.GetFiles(UserExts, "*.dll", SearchOption.AllDirectories));
                }

                if (Directory.Exists(AppExts))
                {
                    exts.AddRange(Directory.GetFiles(UserExts, "*.dll", SearchOption.AllDirectories));
                }

                for (int i = 0; i < exts.Count; i++)
                {
                    string extFile = exts[i];

                    var ext_ass = Assembly.Load(extFile);
                    var ass_types = ext_ass.GetExportedTypes();

                    for (int ass_i = 0; ass_i < ass_types.Length; ass_i++)
                    {
                        var ass_type = ass_types[ass_i];
                        if (ass_type.IsSubclassOf(typeof(BackupActionType)))
                        {
                            current.Add(Activator.CreateInstance(ass_type) as BackupActionType);
                        }
                    }
                }

                BackupActionTypes = current.ToArray();
            }
            return this;
        }
    }
}