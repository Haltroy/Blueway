using LibFoster;
using System;
using System.Diagnostics;
using System.Linq;

namespace Blueway.BackupActions
{
    public class BackupTestActionType : BackupActionType
    {
        public override string Name => "Test";

        public override void ExportAction(Fostrian.FostrianNode node, BackupAction action)
        {
            if (action is BackupTestAction bta)
            {
                node.Add(new Fostrian.FostrianNode(bta.BoolTest, "booltest"));
                node.Add(new Fostrian.FostrianNode(bta.TextTest, "texttest"));
                node.Add(new Fostrian.FostrianNode(bta.PasswordTest, "passwordtest"));
                node.Add(new Fostrian.FostrianNode(BitConverter.ToString(bta.RandomTest), "randomtest"));
                node.Add(new Fostrian.FostrianNode(bta.NumberTest, "numbertest"));
                node.Add(new Fostrian.FostrianNode(bta.DateTest.ToBinary(), "datetest"));
                node.Add(new Fostrian.FostrianNode((long)bta.TimeTest.TotalMilliseconds, "timetest"));
                node.Add(new Fostrian.FostrianNode(bta.OptionTest, "optiontest"));
                node.Add(new Fostrian.FostrianNode(bta.OpenFileTest, "openfiletest"));
                node.Add(new Fostrian.FostrianNode(bta.SaveFileTest, "savefiletest"));
                node.Add(new Fostrian.FostrianNode(bta.DirTest, "dirtest"));
            }
        }

        public override BackupAction GenerateAction() => new BackupTestAction();

        public override BackupActionProperty[] GetProperties()
        {
            BackupActionProperty[] props = new BackupActionProperty[11];

            props[0] = new BackupActionProperty("Boolean Test", "Property to test boolean functionality.", BackupActionPropertyValueType.Boolean, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.BoolTest = v is bool b && b; }
            }, true);

            props[1] = new BackupActionProperty("Text Test", "Property to test text functionality.", BackupActionPropertyValueType.Text, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.TextTest = v is string s ? s : string.Empty; }
            }, "This is a test.");

            props[2] = new BackupActionProperty("Password Test", "Property to test password functionality.", BackupActionPropertyValueType.Password, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.PasswordTest = v is string s ? s : string.Empty; }
            }, "password123");

            props[3] = new BackupActionProperty("Random Bytes Test", "Property to test random byte functionality.", BackupActionPropertyValueType.RandomBytes, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.RandomTest = v is byte[] ba ? ba : new byte[] { 6, 7, 8, 9, 0 }; }
            }, new byte[] { 0, 1, 2, 3, 4, 5 })
            { ByteSize = 6 };

            props[4] = new BackupActionProperty("Number Test", "Property to test number functionality.", BackupActionPropertyValueType.Number, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.NumberTest = v is decimal d ? (int)d : 5; }
            }, 2)
            { Maximum = 10, Minimum = 1, Increment = 1 };

            props[5] = new BackupActionProperty("Date Test", "Property to test date functionality.", BackupActionPropertyValueType.Date, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.DateTest = v is DateTime dt ? dt : DateTime.ParseExact("11/03/2023", "dd/MM/yyyy", null); }
            }, DateTime.ParseExact("11/03/2001", "dd/MM/yyyy", null));

            props[6] = new BackupActionProperty("Time Test", "Property to test date functionality.", BackupActionPropertyValueType.Time, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.TimeTest = v is TimeSpan ts ? ts : TimeSpan.MaxValue; }
            }, TimeSpan.MinValue);
            props[7] = new BackupActionProperty("Option Test", "Property to test option functionality.", BackupActionPropertyValueType.Options, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.OptionTest = v is int i ? i : 0; }
            }, 0, new string[] { "Option 1", "Option 2", "Option 3" });
            props[8] = new BackupActionProperty("Open File Test", "Property to test open file functionality.", BackupActionPropertyValueType.OpenFile, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.OpenFileTest = v is string[] sa ? sa[0] : (v is string s ? s : string.Empty); }
            })
            {
                MultiPick = true,
                DialogTitle = "Open File Test",
                Filters = "Test File|*.test|All Files|*.*"
            };
            props[9] = new BackupActionProperty("Save File Test", "Property to test save file functionality.", BackupActionPropertyValueType.SaveFile, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.SaveFileTest = v is string s ? s : string.Empty; }
            })
            {
                MultiPick = true,
                DialogTitle = "Save File Test",
                Filters = "Test File|*.test|All Files|*.*"
            };
            props[10] = new BackupActionProperty("Open Directory Test", "Property to test open directory functionality.", BackupActionPropertyValueType.Folder, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.DirTest = v is string[] sa ? sa[0] : (v is string s ? s : string.Empty); }
            })
            {
                MultiPick = true,
                DialogTitle = "Folder Test",
            };

            return props;
        }

        public override BackupAction ImportAction(Fostrian.FostrianNode node)
        {
            BackupTestAction bta = new BackupTestAction();

            for (int i = 0; i < node.Size; i++)
            {
                try
                {
                    if (node[i].Type == Fostrian.NodeType.FFF) { continue; }
                    switch (node[i].Name.ToLowerInvariant())
                    {
                        case "booltest":
                            bta.BoolTest = node[i].DataAsBoolean;
                            break;

                        case "texttest":
                            bta.TextTest = node[i].DataAsString;
                            break;

                        case "passwordtest":
                            bta.PasswordTest = node[i].DataAsString;
                            break;

                        case "randomtest":
                            bta.RandomTest = StringToByteArray(node[i].DataAsString);
                            break;

                        case "numbertest":
                            bta.NumberTest = node[i].DataAsInt32;
                            break;

                        case "datetest":
                            bta.DateTest = DateTime.FromBinary(node[i].DataAsInt64);
                            break;

                        case "timetest":
                            bta.TimeTest = TimeSpan.FromMilliseconds(node[i].DataAsInt64);
                            break;

                        case "optiontest":
                            bta.OptionTest = node[i].DataAsInt32;
                            break;

                        case "openfiletest":
                            bta.OpenFileTest = node[i].DataAsString;
                            break;

                        case "savefiletest":
                            bta.SaveFileTest = node[i].DataAsString;
                            break;

                        case "dirtest":
                            bta.DirTest = node[i].DataAsString;
                            break;

                        default:
                            continue;
                    }
                }
                catch (Exception) { continue; }
            }
            return bta;
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }

    public class BackupTestAction : BackupAction
    {
        public bool? BoolTest { get; set; }
        public string TextTest { get; set; }
        public string PasswordTest { get; set; }
        public byte[] RandomTest { get; set; }
        public int NumberTest { get; set; }
        public DateTime DateTest { get; set; }
        public TimeSpan TimeTest { get; set; }
        public int OptionTest { get; set; }
        public string OpenFileTest { get; set; }
        public string SaveFileTest { get; set; }
        public string DirTest { get; set; }
        public override string Name => "Test";

        public override bool WaitForPreviousActions => false;

        public override MultiheadDetails Before(string target, int threadCount)
        {
            Debug.WriteLine($"[TEST ACTION] Before - Target: \"{target}\" Thread Count: {threadCount}");
            return new MultiheadDetails(2, 50, 100);
        }

        public override void Finalize(string target)
        {
            Debug.WriteLine($"[TEST ACTION] Finalize - Target: \"{target}\"");
        }

        public override void Run(string target, bool reverse)
        {
            Progress.Total = 100;
            Progress.Current = 0;
            Debug.WriteLine($"[TEST ACTION] Run (Singular) - Target: \"{target}\" Reverse: {reverse}");
            Debug.WriteLine($"[TEST ACTION] Values: {Environment.NewLine}" +
                $"      BoolTest: {BoolTest} {Environment.NewLine}" +
                $"      TextTest: {TextTest} {Environment.NewLine}" +
                $"      PasswordTest: {PasswordTest} {Environment.NewLine}" +
                $"      RandomTest: {BitConverter.ToString(RandomTest)} {Environment.NewLine}" +
                $"      NumberTest: {NumberTest} {Environment.NewLine}" +
                $"      DateTest: {DateTest:dd MMMM yyyy} {Environment.NewLine}" +
                $"      TimeTest: {TimeTest:g} {Environment.NewLine}" +
                $"      OptionTest: {OptionTest} {Environment.NewLine}" +
                $"      OpenFileTest: {OpenFileTest} {Environment.NewLine}" +
                $"      SaveFileTest: {SaveFileTest} {Environment.NewLine}" +
                $"      DirTest: {DirTest} {Environment.NewLine}");
            Progress.Current = 100;
        }

        public override void Run(string target, bool reverse, int startPoint, int endPoint)
        {
            Progress.IsIndeterminate = true;
            Debug.WriteLine($"[TEST ACTION] Run (Singular) - Target: \"{target}\" Reverse: {reverse} Start Point: {startPoint}  End Point: {endPoint} ");
            Debug.WriteLine($"[TEST ACTION] Values: {Environment.NewLine}" +
                $"      BoolTest: {BoolTest} {Environment.NewLine}" +
                $"      TextTest: {TextTest} {Environment.NewLine}" +
                $"      PasswordTest: {PasswordTest} {Environment.NewLine}" +
                $"      RandomTest: {BitConverter.ToString(RandomTest)} {Environment.NewLine}" +
                $"      NumberTest: {NumberTest} {Environment.NewLine}" +
                $"      DateTest: {DateTest:dd MMMM yyyy} {Environment.NewLine}" +
                $"      TimeTest: {TimeTest:g} {Environment.NewLine}" +
                $"      OptionTest: {OptionTest} {Environment.NewLine}" +
                $"      OpenFileTest: {OpenFileTest} {Environment.NewLine}" +
                $"      SaveFileTest: {SaveFileTest} {Environment.NewLine}" +
                $"      DirTest: {DirTest} {Environment.NewLine}");
        }
    }
}