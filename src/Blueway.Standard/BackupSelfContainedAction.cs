/// <summary>
/// Default backup actions.
/// </summary>
namespace Blueway.BackupActions
{
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