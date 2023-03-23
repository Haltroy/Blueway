namespace Blueway
{
    /// <summary>
    /// Default backup actions.
    /// </summary>
    public static partial class BackupActions
    {
        public class SyncInfo
        {
            // TODO
        }

        public class BackupUploadDownloadAction : BackupAction //SyncInfo
        {
            public override string Name => throw new System.NotImplementedException();

            public override bool WaitForPreviousActions => throw new System.NotImplementedException();

            public override MultiheadDetails Before(string target, int threadCount)
            {
                throw new System.NotImplementedException();
            }

            // TODO
            public override void Finalize(string target)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse, int startPoint, int endPoint)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CommandInfo
        {
            // TODO
        }

        public class BackupExecuteCommand : BackupAction //CommandInfo
        {
            public override string Name => throw new System.NotImplementedException();

            public override bool WaitForPreviousActions => throw new System.NotImplementedException();

            public override MultiheadDetails Before(string target, int threadCount)
            {
                throw new System.NotImplementedException();
            }

            // TODO
            public override void Finalize(string target)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse, int startPoint, int endPoint)
            {
                throw new System.NotImplementedException();
            }
        }

        public class FileInfo
        {
            public FileInfo(string path, string content)
            {
                Path = path ?? throw new System.ArgumentNullException(nameof(path));
                Content = content ?? throw new System.ArgumentNullException(nameof(content));
            }
            public FileInfo(string path) : this(path, "") { }
            public string Path { get; set; }
            public string Content { get; set; }
        }

        public class BackupCreateFileAction : BackupAction //FileInfo
        {
            public override string Name => throw new System.NotImplementedException();

            public override bool WaitForPreviousActions => throw new System.NotImplementedException();

            public override MultiheadDetails Before(string target, int threadCount)
            {
                throw new System.NotImplementedException();
            }

            // TODO
            public override void Finalize(string target)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse, int startPoint, int endPoint)
            {
                throw new System.NotImplementedException();
            }
        }

        public class BackupSelfContainedAction : BackupAction //OperatingSystems.OperatingSystem
        {
            public override string Name => throw new System.NotImplementedException();

            public override bool WaitForPreviousActions => throw new System.NotImplementedException();

            public override MultiheadDetails Before(string target, int threadCount)
            {
                throw new System.NotImplementedException();
            }

            // TODO
            public override void Finalize(string target)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse)
            {
                throw new System.NotImplementedException();
            }

            public override void Run(string target, bool reverse, int startPoint, int endPoint)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
