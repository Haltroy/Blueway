using System;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Blueway
{
    public class BackupHistoryItem
    {
        public BackupHistoryItem(string name, BackupSchema schema, DateTime date, BackupStatus status, DateTime plannedDate, TimeSpan reoccurence, string backupDir)
        {
            Name = name;
            Schema = schema;
            Date = date;
            Status = status;
            PlannedDate = plannedDate;
            Reoccurence = reoccurence;
            BackupDir = backupDir;
        }

        public string Name { get; set; }

        public BackupSchema Schema { get; set; }

        public DateTime Date { get; set; }

        public BackupStatus Status { get; set; }

        public DateTime PlannedDate { get; set; }
        public TimeSpan Reoccurence { get; set; }

        public string BackupDir { get; set; }
    }

    public class BackupSchema
    {
        public BackupSchema(List<BackupAction> actions)
        {
            Actions = actions;
        }

        public List<BackupAction> Actions { get; set; }
    }

    public enum BackupStatus
    {
        Success,
        OnGoing,
        Failure,
        Planned
    }

    /// <summary>
    /// Represents an action that will take on backup process.
    /// </summary>
    public abstract class BackupAction
    {
        /// <summary>
        /// Status of current action.
        /// </summary>
        public BackupStatus Status { get; set; }

        /// <summary>
        /// Arguments of the action.
        /// </summary>
        public object? Args { get; set; }


        /// <summary>
        /// Runs the action from start.
        /// </summary>
        /// <param name="target">Path to backup stuff to.</param>
        /// <param name="reverse"><c>true</c> to apply the backup, otherwise <c>false</c>.</param>
        public abstract void Run(string target, bool reverse);

        /// <summary>
        /// Runs the action from <paramref name="startPoint"/> to <paramref name="endPoint"/>. Used by <see cref="Multihead"/> actions.
        /// </summary>
        /// <param name="target">Path to backup stuff to.</param>
        /// <param name="startPoint">The starting point of the action.</param>
        /// <param name="endPoint">The end point of the action.</param>
        /// <param name="reverse"><c>true</c> to apply the backup, otherwise <c>false</c>.</param>
        public abstract void Run(string target, bool reverse, int startPoint, int endPoint);

        /// <summary>
        /// Event to run after action runs and finishes.
        /// </summary>
        /// <param name="target">Path to backup stuff to.</param>
        public abstract void Finalize(string target);

        /// <summary>
        /// Progress of action.
        /// </summary>
        public Progress Progress { get; set; } = new Progress();

        /// <summary>
        /// Name of the action. Used for translation.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Determines if this action should wait until other actions are finished.
        /// </summary>
        public abstract bool WaitForPreviousActions { get; }

        /// <summary>
        /// Prepares for backup and gets <see cref="MultiheadDetails"/> that holds information about multi-thread operations.
        /// </summary>
        /// <param name="threadCount">Count of threads that are about to spawn on.</param>
        /// <param name="target">Path to backup stuff to.</param>
        /// <returns>see <see cref="MultiheadDetails"/> class properties.</returns>
        public abstract MultiheadDetails Before(string target, int threadCount);
    }

    /// <summary>
    /// Details of a multi-threaded action. Set all to 1 for no multihead.
    /// </summary>
    public class MultiheadDetails
    {
        /// <summary>
        /// Creates a new <see cref="MultiheadDetails"/>. Set all to 1 for no multihead.
        /// </summary>
        /// <param name="pieceCount">Count of pieces, or threads. Set this to 1 for no multihead.</param>
        /// <param name="pieceSize">Size of a piece that will be processed by one thread. Set this to 1 for no multihead.</param>
        /// <param name="totalPieceSize">Total sizes of all pieces, processed by <paramref name="pieceCount"/> of threads. Set this to 1 for no multihead.</param>
        public MultiheadDetails(int pieceCount, int pieceSize, int totalPieceSize)
        {
            PieceCount = pieceCount;
            PieceSize = pieceSize;
            TotalPieceSize = totalPieceSize;
        }

        /// <summary>
        /// Count of pieces, or threads. Set this to 1 for no multihead.
        /// </summary>
        public int PieceCount { get; }

        /// <summary>
        /// Size of a piece that will be processed by one thread. Set this to 1 for no multihead.
        /// </summary>
        public int PieceSize { get; }

        /// <summary>
        /// Total sizes of all pieces, processed by <see cref="PieceCount"/> of threads. Set this to 1 for no multihead.
        /// </summary>
        public int TotalPieceSize { get; }
    }

    /// <summary>
    /// Progress of a <see cref="BackupAction{T}"/>.
    /// </summary>
    public class Progress
    {
        /// <summary>
        /// Percentage of the progress.
        /// </summary>
        public double Percentage => (double)Current / (double)Total;

        /// <summary>
        /// Current position of the progress.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Total size of the progress.
        /// </summary>
        public int Total { get; set; }
    }
}
