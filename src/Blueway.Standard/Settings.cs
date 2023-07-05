using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
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
        public List<AppSource> Sources { get; set; } = new List<AppSource>();

        public List<Theme> Themes { get; set; } = new List<Theme>() { DefaultThemes.Light, DefaultThemes.Dark, DefaultThemes.Breath, DefaultThemes.Breeze, DefaultThemes.Backupster };

        public List<BackupHistoryItem> History { get; set; } = new List<BackupHistoryItem>();

        public BackupHistoryItem GetLatestOrEmptyBackup()
        {
            return GetLatestBackup() ?? new BackupHistoryItem();
        }

        public BackupHistoryItem[] AutoBackups => GetAutoBackups();

        public BackupHistoryItem[] GetAutoBackups()
        {
            List<BackupHistoryItem> items = new List<BackupHistoryItem>();
            for (int i = 0; i < History.Count; i++)
            {
                if (History[i].Schedule.IsNotEmpty)
                {
                    items.Add(History[i]);
                }
            }
            return items.ToArray();
        }

        public Settings AutoLoadHistory()
        {
            if (File.Exists(UserHistoryFile))
            {
                ParseHistory(UserHistoryFile);
                return this;
            }
            if (File.Exists(AppHistoryFile))
            {
                ParseHistory(AppHistoryFile);
            }
            return this;
        }

        private void ParseHistory(string file)
        {
            if (new FileInfo(file).Length <= 0)
            {
                return;
            }
            var root = Fostrian.Parse(file);
            for (int i = 0; i < root.Size; i++)
            {
                History.Add(ParseHistoryNode(root[i]));
            }
        }

        private BackupHistoryItem ParseHistoryNode(Fostrian.FostrianNode node)
        {
            BackupHistoryItem backup = new BackupHistoryItem();
            backup.Schema.Name = node.DataAsString;
            for (int i = 0; i < node.Size; i++)
            {
                try
                {
                    if (node[i].Type == Fostrian.NodeType.FFF) { continue; }
                    switch (node[i].Name.ToLowerInvariant())
                    {
                        case "dir":
                            backup.BackupDir = node[i].DataAsString;
                            break;

                        case "status":
                            backup.Status = (BackupStatus)node[i].DataAsInt16;
                            break;

                        case "date":
                            backup.Date = DateTime.FromBinary(node[i].DataAsInt64);
                            break;

                        case "schedule":
                            backup.Schema.Schedule = ScheduleInfo.FromBinary(node[i].DataAsInt64);
                            break;

                        case "schema":
                            ParseHistoryItemSchema(backup, node[i]);
                            break;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return backup;
        }

        private void ParseHistoryItemSchema(BackupHistoryItem backup, Fostrian.FostrianNode root)
        {
            for (int i = 0; i < root.Size; i++)
            {
                for (int si = 0; si < BackupActionTypes.Length; i++)
                {
                    if (string.Equals(BackupActionTypes[si].Name, root[i].DataAsString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        backup.Schema.Actions.Add(BackupActionTypes[si].ImportAction(root[i]));
                        break;
                    }
                }
            }
        }

        public BackupHistoryItem GetLatestBackup()
        {
            if (History.Count > 0)
            {
                History = History.OrderBy(it => it.Date).ToList();
                return History.Last();
            }
            else
            {
                return null;
            }
        }

        #endregion Settings

        public Settings()
        {
            if (!Directory.Exists(UserFiles))
            {
                Directory.CreateDirectory(UserFiles);
            }
            BackupActionTypes = new BackupActionType[]
            {
#if DEBUG
                new BackupActions.BackupTestActionType(),
#endif
                new BackupActions.BackupDirectoryActionType(),
                new BackupActions.BackupCreateFileActionType(),
                new BackupActions.BackupExecuteCommandActionType(),
                new BackupActions.BackupSelfContainedActionType(),
                new BackupActions.BackupUploadDownloadActionType()
            };
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

        public Settings SaveHistory(string saveTo = null)
        {
            var root = Fostrian.GenerateRootNode(false);
            root.Encoding = Encoding.Unicode;
            root.StartByte = 0x02;
            root.EndByte = 0x03;
            for (int i = 0; i < History.Count; i++)
            {
                var item = History[i];
                var node = root.Add(item.Name);
                node.Type = Fostrian.NodeType.FVF;
                node.Add(new Fostrian.FostrianNode(item.BackupDir, "dir"));
                node.Add(new Fostrian.FostrianNode((short)item.Status, "status"));
                node.Add(new Fostrian.FostrianNode(item.Date.ToBinary(), "date"));
                node.Add(new Fostrian.FostrianNode(item.Schedule.ToBinary(), "schedule"));
                var schema = node.Add(new Fostrian.FostrianNode(true, "schema"));
                for (int ai = 0; ai < item.Schema.Actions.Count; ai++)
                {
                    var action = item.Schema.Actions[ai];
                    var actionNode = schema.Add(action.Name);
                    for (int si = 0; si < BackupActionTypes.Length; si++)
                    {
                        if (string.Equals(BackupActionTypes[si].Name, action.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            BackupActionTypes[si].ExportAction(actionNode, action);
                            break;
                        }
                    }
                }
            }
            Fostrian.Recreate(root, saveTo ?? UserHistoryFile);
            return this;
        }

        public Settings SaveConfig(string saveTo = null)
        {
            var root = Fostrian.GenerateRootNode(false);
            root.Encoding = Encoding.Unicode;
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

            LoadThemes();

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

        private Settings LoadThemes()
        {
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
            return this;
        }

        public void RegisterBackupActionType(BackupActionType action)
        {
            BackupActionType[] _bat = BackupActionTypes;
            Array.Resize(ref _bat, _bat.Length + 1);
            _bat[_bat.Length - 1] = action;
            BackupActionTypes = _bat;
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

                    // do not laugh
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