﻿/// <summary>
/// Default backup actions.
/// </summary>
namespace Blueway.BackupActions
{
    public class FileInfo
    {
        public FileInfo(string path, string content)
        {
            Path = path ?? throw new System.ArgumentNullException(nameof(path));
            Content = content ?? throw new System.ArgumentNullException(nameof(content));
        }

        public FileInfo(string path) : this(path, "")
        {
        }

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
}