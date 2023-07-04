using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;

namespace Blueway.Views
{
    public partial class BackupCustomize : AUC
    {
        public BackupCustomize()
        {
            InitializeComponent();
            Initialized += (s, e) => LoadActions();
        }

        private AUC? GoBackTo;
        private BackupSchema Schema { get; set; } = new(new());

        public override AUC? ReturnTo(MainWindow.Buttons buttons) => GoBackTo;

        internal BackupCustomize LoadSchema(BackupSchema backupSchema)
        {
            for (int i = 0; i < backupSchema.Actions.Count; i++)
            {
                ActionList.Children.Add(GenerateFromAction(backupSchema.Actions[i], backupSchema.Actions[i].GetBackupActionType));
            }
            return this;
        }

        internal BackupCustomize LoadSchema(BackupHistoryItem item)
        {
            return LoadSchema(item);
        }

        public BackupCustomize GoBackToAUC(AUC auc)
        {
            GoBackTo = auc;
            return this;
        }

        private Control GenerateFromAction(BackupAction action, BackupActionType type)
        {
            Border border = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, CornerRadius = new Avalonia.CornerRadius(10), BorderThickness = new Avalonia.Thickness(10), [!BackgroundProperty] = SchemaName[!BackgroundProperty] };
            StackPanel btPanel = new() { Spacing = 5 };
            border.Child = btPanel;

            // HEAD
            // TODO: add translation here
            btPanel.Children.Add(new TextBlock() { Text = action.Name, FontSize = 20, FontWeight = Avalonia.Media.FontWeight.Bold });
            btPanel.Children.Add(new TextBlock() { Text = "DESC OF ACTION HERE" /* TranslationSystem.GetDesc(action.Name) */ });

            StackPanel btPanelOptions = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
            btPanel.Children.Add(btPanelOptions);
            Button moveUp = new() { Content = new TextBlock() { Text = "Move Up" } };
            moveUp.Click += (s, e) => { };
            btPanelOptions.Children.Add(moveUp);
            Button moveDown = new() { Content = new TextBlock() { Text = "Move Down" } };
            moveDown.Click += (s, e) => { };
            btPanelOptions.Children.Add(moveDown);
            Button delete = new() { Content = new TextBlock() { Text = "Delete" } };
            delete.Click += (s, e) => { };
            btPanelOptions.Children.Add(delete);

            // SHOW MORE
            Button btShowMore = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
            Grid showMore = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto,*") };
            showMore.Children.Add(new Separator());
            showMore.Children.Add(new TextBlock() { FontSize = 10, Text = "Click to show/hide details" });
            showMore.Children.Add(new Separator());
            Grid.SetColumn(showMore.Children[0], 0);
            Grid.SetColumn(showMore.Children[1], 1);
            Grid.SetColumn(showMore.Children[2], 2);
            btShowMore.Content = showMore;
            btPanel.Children.Add(btShowMore);

            StackPanel editPanel = new() { IsVisible = false, Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 15 };
            btPanel.Children.Add(editPanel);

            btShowMore.Click += (s, e) =>
            {
                editPanel.IsVisible = !editPanel.IsVisible;
            };

            var props = type.GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    if (props[i] is null) { continue; }
                    var prop = props[i];
                    switch (prop.ValueType)
                    {
                        case BackupActionPropertyValueType.Boolean:
                            StackPanel spBool1 = new();
                            StackPanel spBool = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spBool.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            ToggleSwitch swBool = new() { IsChecked = prop.DefaultValue is bool b && b };
                            swBool.IsCheckedChanged += (s, e) => prop.PerformChange(action, swBool.IsChecked);
                            spBool.Children.Add(swBool);
                            spBool1.Children.Add(spBool);
                            spBool1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spBool1);
                            break;

                        default:
                        case BackupActionPropertyValueType.Text:
                            StackPanel spText1 = new();
                            DockPanel spText = new() { LastChildFill = true };
                            spText.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Avalonia.Thickness(0, 0, 5, 0) });
                            DockPanel.SetDock(spText.Children[0], Dock.Left);
                            TextBox tbText = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = prop.DefaultValue is string str ? str : (prop.DefaultValue != null ? prop.DefaultValue.ToString() : string.Empty) };
                            tbText.PropertyChanged += (s, e) => { if (e.Property == TextBlock.TextProperty) { prop.PerformChange(action, tbText.Text); } };
                            spText.Children.Add(tbText);
                            spText1.Children.Add(spText);
                            spText1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spText1);
                            break;

                        case BackupActionPropertyValueType.Password:
                            StackPanel spPassword1 = new();
                            DockPanel spPassword = new();
                            spPassword.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Avalonia.Thickness(0, 0, 5, 0) });
                            DockPanel.SetDock(spPassword.Children[0], Dock.Left);
                            TextBox tbPassword = new()
                            {
                                PasswordChar = '#',
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                Text = prop.DefaultValue is string pwd ? pwd : (prop.DefaultValue != null ? prop.DefaultValue.ToString() : string.Empty)
                            };
                            tbPassword.PropertyChanged += (s, e) => { if (e.Property == TextBlock.TextProperty) { prop.PerformChange(action, tbPassword.Text); } };

                            Button btPasswordShowHide = new() { Content = new TextBlock() { Text = "Show/Hide" }, Margin = new Avalonia.Thickness(5, 0, 0, 0) };
                            DockPanel.SetDock(btPasswordShowHide, Dock.Right);
                            btPasswordShowHide.Click += (s, e) => { tbPassword.RevealPassword = !tbPassword.RevealPassword; };
                            spPassword.Children.Add(btPasswordShowHide);
                            spPassword.Children.Add(tbPassword);
                            spPassword1.Children.Add(spPassword);
                            spPassword1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spPassword1);
                            break;

                        case BackupActionPropertyValueType.RandomBytes:
                            StackPanel spRandom1 = new();
                            StackPanel spRandom = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spRandom.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            TextBlock tbInfo = new() { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Text = BitConverter.ToString(prop.DefaultValue is byte[] ba ? ba : Array.Empty<byte>()) };
                            Button btGen = new() { Content = new TextBlock() { Text = "Generate" } };
                            btGen.Click += (s, e) =>
                            {
                                byte[] newSeq = new byte[prop.ByteSize];
                                for (int i = 0; i < newSeq.Length; i++)
                                {
                                    newSeq[i] = (byte)new Random().Next(byte.MinValue, byte.MaxValue);
                                }
                                prop.PerformChange(action, newSeq);
                                tbInfo.Text = BitConverter.ToString(newSeq);
                            };
                            spRandom.Children.Add(btGen);
                            spRandom.Children.Add(tbInfo);
                            spRandom1.Children.Add(spRandom);
                            spRandom1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spRandom1);
                            break;

                        case BackupActionPropertyValueType.Number:
                            StackPanel spNumber1 = new();
                            StackPanel spNumber = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spNumber.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            NumericUpDown nudNumber = new() { Maximum = prop.Maximum, Minimum = prop.Minimum, Increment = prop.Increment, Value = prop.DefaultValue is decimal dec ? dec : 0 };
                            nudNumber.ValueChanged += (s, e) => prop.PerformChange(action, nudNumber.Value);
                            spNumber.Children.Add(nudNumber);
                            spNumber1.Children.Add(spNumber);
                            spNumber1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spNumber1);
                            break;

                        case BackupActionPropertyValueType.Date:
                            StackPanel spDate1 = new();
                            StackPanel spDate = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spDate.Children.Add(new TextBlock() { Text = prop.Name + ":" });
                            DatePicker dpDate = new() { SelectedDate = new DateTimeOffset(prop.DefaultValue is DateTime dt ? dt : DateTime.Now) };
                            dpDate.SelectedDateChanged += (s, e) => prop.PerformChange(action, dpDate.SelectedDate.HasValue ? dpDate.SelectedDate.Value.DateTime : DateTime.Now);
                            spDate.Children.Add(dpDate);
                            spDate1.Children.Add(spDate);
                            spDate1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spDate1);
                            break;

                        case BackupActionPropertyValueType.Time:
                            StackPanel spTime1 = new();
                            StackPanel spTime = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spTime.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            TimePicker dpTime = new() { SelectedTime = prop.DefaultValue is TimeSpan ts ? ts : TimeSpan.Zero };
                            dpTime.SelectedTimeChanged += (s, e) => prop.PerformChange(action, dpTime.SelectedTime ?? TimeSpan.Zero);
                            spTime.Children.Add(dpTime);
                            spTime1.Children.Add(spTime);
                            spTime1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spTime1);
                            break;

                        case BackupActionPropertyValueType.Options:
                            StackPanel spOptions1 = new();
                            StackPanel spOptions = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                            spOptions.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            ComboBox cbOptions = new() { SelectedIndex = prop.DefaultValue is int opi ? opi : 0 };
                            for (int oi = 0; oi < prop.Options.Length; oi++)
                            {
                                cbOptions.Items.Add(new ComboBoxItem() { Content = new TextBlock() { Text = prop.Options[oi] } });
                            }
                            cbOptions.SelectionChanged += (s, e) => prop.PerformChange(action, cbOptions.SelectedIndex);
                            spOptions.Children.Add(cbOptions);
                            spOptions1.Children.Add(spOptions);
                            spOptions1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spOptions1);
                            break;

                        case BackupActionPropertyValueType.OpenFile:
                            StackPanel spOFile1 = new();
                            DockPanel spOFile = new() { LastChildFill = true };
                            spOFile.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Avalonia.Thickness(0, 0, 5, 0) });
                            DockPanel.SetDock(spOFile.Children[0], Dock.Left);
                            TextBox tbOFile = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = prop.DefaultValue is string ofile ? ofile : (prop.DefaultValue != null ? prop.DefaultValue.ToString() : string.Empty) };
                            tbOFile.PropertyChanged += (s, e) => { if (e.Property == TextBlock.TextProperty) { prop.PerformChange(action, tbOFile.Text); } };
                            Button btOFile = new() { Content = new TextBlock() { Text = "..." }, Margin = new Avalonia.Thickness(5, 0, 0, 0) };
                            DockPanel.SetDock(btOFile, Dock.Right);
                            System.Collections.Generic.List<Avalonia.Platform.Storage.FilePickerFileType> o_filetypes = new();
                            var _oft = prop.Filters.Split('|');
                            for (int ofi = 0; ofi < _oft.Length; ofi += 2)
                            {
                                if (_oft.Length > ofi + 1)
                                {
                                    o_filetypes.Add(new Avalonia.Platform.Storage.FilePickerFileType(_oft[ofi]) { Patterns = _oft[ofi + 1].Split(';') });
                                }
                            }
                            btOFile.Click += async (s, e) =>
                            {
                                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                                {
                                    if (MainWindow != null && MainWindow.StorageProvider.CanOpen)
                                    {
                                        var files = await MainWindow.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions() { Title = prop.DialogTitle, FileTypeFilter = o_filetypes, AllowMultiple = prop.MultiPick });
                                        if (files != null && files.Count > 0)
                                        {
                                            var _files = new string[files.Count];
                                            for (int fi = 0; fi < _files.Length; fi++)
                                            {
                                                _files[fi] = files[fi].Path.AbsolutePath;
                                            }
                                            prop.PerformChange(action, _files);
                                        }
                                    }
                                });
                            };
                            spOFile.Children.Add(btOFile);
                            spOFile.Children.Add(tbOFile);
                            spOFile1.Children.Add(spOFile);
                            spOFile1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spOFile1);
                            break;

                        case BackupActionPropertyValueType.SaveFile:
                            StackPanel spSFile1 = new();
                            DockPanel spSFile = new() { LastChildFill = true };
                            spSFile.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
                            DockPanel.SetDock(spSFile.Children[0], Dock.Left);
                            TextBox tbSFile = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = prop.DefaultValue is string sfile ? sfile : (prop.DefaultValue != null ? prop.DefaultValue.ToString() : string.Empty), Margin = new Avalonia.Thickness(0, 0, 5, 0) };
                            tbSFile.PropertyChanged += (s, e) => { if (e.Property == TextBlock.TextProperty) { prop.PerformChange(action, tbSFile.Text); } };
                            Button btSFile = new() { Content = new TextBlock() { Text = "..." }, Margin = new Avalonia.Thickness(5, 0, 0, 0) };
                            DockPanel.SetDock(btSFile, Dock.Right);
                            System.Collections.Generic.List<Avalonia.Platform.Storage.FilePickerFileType> s_filetypes = new();
                            var _sft = prop.Filters.Split('|');
                            for (int sfi = 0; sfi < _sft.Length; sfi += 2)
                            {
                                if (_sft.Length > sfi + 1)
                                {
                                    s_filetypes.Add(new Avalonia.Platform.Storage.FilePickerFileType(_sft[sfi]) { Patterns = _sft[sfi + 1].Split(';') });
                                }
                            }
                            btSFile.Click += async (s, e) =>
                            {
                                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                                {
                                    if (MainWindow != null && MainWindow.StorageProvider.CanSave)
                                    {
                                        var files = await MainWindow.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions() { Title = prop.DialogTitle, FileTypeChoices = s_filetypes });
                                        if (files != null)
                                        {
                                            prop.PerformChange(action, files.Path.AbsolutePath);
                                        }
                                    }
                                });
                            };

                            spSFile.Children.Add(btSFile);
                            spSFile.Children.Add(tbSFile);
                            spSFile1.Children.Add(spSFile);
                            spSFile1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spSFile1);
                            break;

                        case BackupActionPropertyValueType.Folder:
                            StackPanel spDir1 = new();
                            DockPanel spDir = new() { LastChildFill = true };
                            spDir.Children.Add(new TextBlock() { Text = prop.Name + ":", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Avalonia.Thickness(0, 0, 5, 0) });
                            DockPanel.SetDock(spDir.Children[0], Dock.Left);
                            TextBox tbDir = new() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = prop.DefaultValue is string file ? file : (prop.DefaultValue != null ? prop.DefaultValue.ToString() : string.Empty) };
                            tbDir.PropertyChanged += (s, e) => { if (e.Property == TextBlock.TextProperty) { prop.PerformChange(action, tbDir.Text); } };
                            Button btDir = new() { Content = new TextBlock() { Text = "..." }, Margin = new Avalonia.Thickness(5, 0, 0, 0) };
                            DockPanel.SetDock(btDir, Dock.Right);
                            btDir.Click += async (s, e) =>
                            {
                                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                                {
                                    if (MainWindow != null && MainWindow.StorageProvider.CanPickFolder)
                                    {
                                        var files = await MainWindow.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions() { Title = prop.DialogTitle, AllowMultiple = prop.MultiPick });
                                        if (files != null && files.Count > 0)
                                        {
                                            var _files = new string[files.Count];
                                            for (int fi = 0; fi < _files.Length; fi++)
                                            {
                                                _files[fi] = files[fi].Path.AbsolutePath;
                                            }
                                            prop.PerformChange(action, _files);
                                        }
                                    }
                                });
                            };
                            spDir.Children.Add(btDir);
                            spDir.Children.Add(tbDir);
                            spDir1.Children.Add(spDir);
                            spDir1.Children.Add(new TextBlock() { Text = prop.Description });
                            editPanel.Children.Add(spDir1);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    StackPanel exPanel = new() { Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 5 };
                    exPanel.Children.Add(new TextBlock() { Text = $"A(n) {ex.GetType().FullName} error occurred during loading one property:" });
                    exPanel.Children.Add(new TextBox() { IsReadOnly = true, Text = ex.ToString() });
                    editPanel.Children.Add(exPanel);
                    continue;
                }
            }

            return border;
        }

        private BackupCustomize LoadActions()
        {
            if (Settings != null)
            {
                MenuFlyout mfo = new();
                AddNew.Flyout = mfo;

                for (int i = 0; i < Settings.BackupActionTypes.Length; i++)
                {
                    MenuItem item = new() { Header = Settings.BackupActionTypes[i].Name, Tag = Settings.BackupActionTypes[i] };
                    mfo.Items.Add(item);
                    item.Click += (s, e) =>
                    {
                        if (item.Tag is BackupActionType bat)
                        {
                            var action = bat.GenerateAction();
                            Schema.Actions.Add(action);
                            ActionList.Children.Add(GenerateFromAction(action, bat));
                        }
                    };
                }
            }
            return this;
        }

        private async void BrowseFolder(object? s, RoutedEventArgs e)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (MainWindow != null && MainWindow.StorageProvider.CanPickFolder)
                {
                    // TODO: Add translations
                    var files = await MainWindow.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = false, Title = "Open a folder..." });
                    if (files != null && files.Count > 0)
                    {
                        BackupTo.Text = files[0].Path.AbsolutePath;
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Input);
        }

        public void StartBackup(object? s, RoutedEventArgs e)
        {
            MainWindow?.SwitchTo(new BackupProcess().LoadSchema(Schema));
        }

        private void OpenFlyout(object? s, RoutedEventArgs e)
        {
            if (s == null) { return; }
            if (s is Button btn && btn.Flyout is FlyoutBase flyout)
            {
                flyout.ShowAt(btn);
            }
        }

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.Back;
    }
}