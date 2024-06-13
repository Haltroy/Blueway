using LibFoster;

/// <summary>
/// Default backup actions.
/// </summary>
namespace Blueway.BackupActions
{
    // TODO
    public class BackupSelfContainedActionType : BackupActionType
    {
        public override string Name => "SelfContained";

        public override void ExportAction(Fostrian.FostrianNode node, BackupAction action)
        {
            throw new System.NotImplementedException();
        }

        public override BackupAction GenerateAction()
        {
            throw new System.NotImplementedException();
        }

        public override BackupActionProperty[] GetProperties()
        {
            throw new System.NotImplementedException();
        }

        public override BackupAction ImportAction(Fostrian.FostrianNode node)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BackupSelfContainedAction : BackupAction //OperatingSystems.OperatingSystem
    {
        public override string Name => "SelfContained";

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