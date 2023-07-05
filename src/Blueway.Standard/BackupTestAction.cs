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
                if (bta.BoolTest != true)
                {
                    node.Add(new Fostrian.FostrianNode(bta.BoolTest, "booltest"));
                }
                if (!string.Equals(bta.TextTest, "This is a test."))
                {
                    node.Add(new Fostrian.FostrianNode(bta.TextTest, "texttest"));
                }
                if (!string.Equals(bta.PasswordTest, "password123"))
                {
                    node.Add(new Fostrian.FostrianNode(bta.PasswordTest, "passwordtest"));
                }
                if (RandomSeqIsDefault(bta.RandomTest, new byte[] { 0, 1, 2, 3, 4, 5 }))
                {
                    node.Add(new Fostrian.FostrianNode(BitConverter.ToString(bta.RandomTest), "randomtest"));
                }
                if (bta.NumberTest != 2)
                {
                    node.Add(new Fostrian.FostrianNode(bta.NumberTest, "numbertest"));
                }
                if (DateTime.Equals(bta.DateTest, DateTime.ParseExact("11/03/2001", "dd/MM/yyyy", null)))
                {
                    node.Add(new Fostrian.FostrianNode(bta.DateTest.ToBinary(), "datetest"));
                }

                if (TimeSpan.Equals(bta.TimeTest, TimeSpan.FromMinutes(1)))
                {
                    node.Add(new Fostrian.FostrianNode((long)bta.TimeTest.TotalMilliseconds, "timetest"));
                }

                if (bta.OptionTest != 0)
                {
                    node.Add(new Fostrian.FostrianNode(bta.OptionTest, "optiontest"));
                }

                if (!string.Equals(bta.OpenFileTest, string.Empty))
                {
                    node.Add(new Fostrian.FostrianNode(bta.OpenFileTest, "openfiletest"));
                }

                if (!string.Equals(bta.SaveFileTest, string.Empty))
                {
                    node.Add(new Fostrian.FostrianNode(bta.SaveFileTest, "savefiletest"));
                }

                if (!string.Equals(bta.DirTest, string.Empty))
                {
                    node.Add(new Fostrian.FostrianNode(bta.DirTest, "dirtest"));
                }
            }
        }

        private bool RandomSeqIsDefault(byte[] arr, byte[] def)
        {
            if (arr.Length > def.Length || def.Length > arr.Length)
            {
                return false;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                if (def[i] != arr[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override BackupAction GenerateAction() => new BackupTestAction();

        public override BackupActionProperty[] GetProperties()
        {
            BackupActionProperty[] props = new BackupActionProperty[11];

            props[0] = new BackupActionProperty("Boolean Test", "Property to test boolean functionality.", BackupActionPropertyValueType.Boolean, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.BoolTest = v is bool b && b; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.BoolTest; } else { return false; }
            }, true);

            props[1] = new BackupActionProperty("Text Test", "Property to test text functionality.", BackupActionPropertyValueType.Text, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.TextTest = v is string s ? s : string.Empty; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.TextTest; } else { return string.Empty; }
            }, "This is a test.");

            props[2] = new BackupActionProperty("Password Test", "Property to test password functionality.", BackupActionPropertyValueType.Password, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.PasswordTest = v is string s ? s : string.Empty; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.PasswordTest; } else { return string.Empty; }
            }, "password123");

            props[3] = new BackupActionProperty("Random Bytes Test", "Property to test random byte functionality.", BackupActionPropertyValueType.RandomBytes, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.RandomTest = v is byte[] ba ? ba : new byte[] { 6, 7, 8, 9, 0 }; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.RandomTest; } else { return Array.Empty<byte>(); }
            }, new byte[] { 0, 1, 2, 3, 4, 5 })
            { ByteSize = 6 };

            props[4] = new BackupActionProperty("Number Test", "Property to test number functionality.", BackupActionPropertyValueType.Number, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.NumberTest = v is decimal d ? (int)d : 5; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.NumberTest; } else { return 0; }
            }, 2)
            { Maximum = 10, Minimum = 1, Increment = 1 };

            props[5] = new BackupActionProperty("Date Test", "Property to test date functionality.", BackupActionPropertyValueType.Date, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.DateTest = v is DateTime dt ? dt : DateTime.ParseExact("11/03/2023", "dd/MM/yyyy", null); }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.DateTest; } else { return DateTime.Now; }
            }, DateTime.ParseExact("11/03/2001", "dd/MM/yyyy", null));

            props[6] = new BackupActionProperty("Time Test", "Property to test date functionality.", BackupActionPropertyValueType.Time, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.TimeTest = v is TimeSpan ts ? ts : TimeSpan.MaxValue; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.TimeTest; } else { return TimeSpan.FromMinutes(1); }
            }, TimeSpan.FromMinutes(1));
            props[7] = new BackupActionProperty("Option Test", "Property to test option functionality.", BackupActionPropertyValueType.Options, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.OptionTest = v is int i ? i : 0; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.OptionTest; } else { return 0; }
            }, 0, new string[] { "Option 1", "Option 2", "Option 3" });
            props[8] = new BackupActionProperty("Open File Test", "Property to test open file functionality.", BackupActionPropertyValueType.OpenFile, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.OpenFileTest = v is string[] sa ? sa[0] : (v is string s ? s : string.Empty); }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.OpenFileTest; } else { return string.Empty; }
            })
            {
                MultiPick = true,
                DialogTitle = "Open File Test",
                Filters = "Test File|*.test|All Files|*.*"
            };
            props[9] = new BackupActionProperty("Save File Test", "Property to test save file functionality.", BackupActionPropertyValueType.SaveFile, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.SaveFileTest = v is string s ? s : string.Empty; }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.SaveFileTest; } else { return string.Empty; }
            })
            {
                MultiPick = true,
                DialogTitle = "Save File Test",
                Filters = "Test File|*.test|All Files|*.*"
            };
            props[10] = new BackupActionProperty("Open Directory Test", "Property to test open directory functionality.", BackupActionPropertyValueType.Folder, (a, v) =>
            {
                if (a is BackupTestAction bta) { bta.DirTest = v is string[] sa ? sa[0] : (v is string s ? s : string.Empty); }
            }, (a) =>
            {
                if (a is BackupTestAction bta) { return bta.DirTest; } else { return string.Empty; }
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
        public bool? BoolTest { get; set; } = true;
        public string TextTest { get; set; } = "This is a test.";
        public string PasswordTest { get; set; } = "password123";
        public byte[] RandomTest { get; set; } = new byte[] { 1, 2, 3, 4, 5 };
        public int NumberTest { get; set; } = 2;
        public DateTime DateTest { get; set; } = DateTime.ParseExact("11/03/2001", "dd/MM/yyyy", null);
        public TimeSpan TimeTest { get; set; } = TimeSpan.FromMinutes(1);
        public int OptionTest { get; set; } = 0;
        public string OpenFileTest { get; set; } = string.Empty;
        public string SaveFileTest { get; set; } = string.Empty;
        public string DirTest { get; set; } = string.Empty;
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