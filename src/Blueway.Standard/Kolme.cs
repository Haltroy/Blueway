using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

// TODO: For downloads and uploading, use Aria2 https://github.com/rogerfar/Aria2.NET

namespace ProjectKolme
{
    public class BackupHistoryItem
    {
        public string Name { get; set; }

        public BackupSchema Schema { get; set; }

        public DateTime Date { get; set; }

        public BackupStatus Status { get; set; }

        public DateTime PlannedDate { get; set; }
        public TimeSpan Reoccurence { get; set; }

        public string BackupDir { get; set; }

        // public BackupContent Content { get; set; }
    }

    public class BackupSchema
    {
    }

    public enum BackupStatus
    {
        Success, OnGoing, Failure, Planned
    }

    /// <summary>
    /// Represents an action that will take on backup process.
    /// </summary>
    /// <typeparam name="T">Type of action. Example: <see cref="string"/> for files and directories, <see cref="Uri"/> to upload/download stuff etc.</typeparam>
    public abstract class BackupAction<T>
    {
        /// <summary>
        /// Status of current action.
        /// </summary>
        public BackupStatus Status { get; set; }

        /// <summary>
        /// Arguments of the action.
        /// </summary>
        public T[] Args { get; set; }

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
        /// Progress of action.
        /// </summary>
        public Progress Progress { get; set; }

        /// <summary>
        /// Name of the action. Used for translation.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Determines if this action should wait until other actions are finished.
        /// </summary>
        public abstract bool WaitForPreviousActions { get; }

        /// <summary>
        /// Determines if this action can be called from <see cref="Run(string, int, int)"/> for multi-thread operation.
        /// </summary>
        public abstract bool Multihead { get; }

        /// <summary>
        /// Gets <see cref="MultiheadDetails"/> that hols information about multi-thread operations.
        /// </summary>
        /// <param name="threadCount">Count of threads that are about to spwan on.</param>
        /// <returns>see <see cref="MultiheadDetails"/> class properties.</returns>
        public abstract MultiheadDetails GetMultiheadDetails(int threadCount);

        /// <summary>
        /// Schema related to this action.
        /// </summary>
        public BackupSchema Schema { get; }
    }

    /// <summary>
    /// Details of a multi-threaded action.
    /// </summary>
    public class MultiheadDetails
    {
        /// <summary>
        /// Creates a new <see cref="MultiheadDetails"/>.
        /// </summary>
        /// <param name="pieceCount">Count of pieces, or threads.</param>
        /// <param name="pieceSize">Size of a piece that will be processed by one thread.</param>
        /// <param name="totalPieceSize">Total sizes of all pieces, processed by <paramref name="pieceCount"/> of threads.</param>
        public MultiheadDetails(int pieceCount, int pieceSize, int totalPieceSize)
        {
            PieceCount = pieceCount;
            PieceSize = pieceSize;
            TotalPieceSize = totalPieceSize;
        }

        /// <summary>
        /// Count of pieces, or threads.
        /// </summary>
        public int PieceCount { get; }

        /// <summary>
        /// Size of a piece that will be processed by one thread.
        /// </summary>
        public int PieceSize { get; }

        /// <summary>
        /// Total sizes of all pieces, processed by <see cref="PieceCount"/> of threads.
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

    /// <summary>
    /// Default backup actions.
    /// </summary>
    public static class BackupActions
    {
        /// <summary>
        /// Backups a directory with all the files.
        /// </summary>
        public class BackupDirectoryAction : BackupAction<BackupDirectoryInfo>
        {
            public override string Name => "Dir";

            public override bool WaitForPreviousActions => true;

            public override bool Multihead => true;

            public override MultiheadDetails GetMultiheadDetails(int threadCount)
            {
                // TODO: Search directory and put the list somewhere, then divide the list count. Also set Progress.Total here.
                throw new NotImplementedException();
            }

            public override void Run(string target, bool reverse)
            {
                // TODO: just copy all files.
                throw new NotImplementedException();
            }

            public override void Run(string target, bool reverse, int startPoint, int endPoint)
            {
                // TODO: Using that list mentioned up in Multihead Details event, start copying files. NOTE: add to progress not set it.
                // Progress.Current++;
                throw new NotImplementedException();
            }

            /// <summary>
            /// Algorithms to use when getting hashes for files for later control. These hashes will be noted to a file in the root directory of the backup target directory.
            /// </summary>
            public HashAlgoritm Algoritm { get; set; }

            /// <summary>
            /// Salts the hashes so their outputs will be different than regular hashing. Will generate a salt file with the <see cref="HashSalt"/> data.
            /// </summary>
            public bool HashUseSalt { get; set; }

            /// <summary>
            /// Salts to use for hashing if <see cref="HashUseSalt"/> is enabled.
            /// </summary>
            public byte[] HashSalt { get; set; }

            public enum HashAlgoritm
            {
                /// <summary>
                /// Don't include hashes.
                /// </summary>
                None,

                /// <summary>
                /// Uses message-digest algorithm to get file hashes, used for verifying files in a disk or to see if the file is changed or not before attempting a backup to save time.
                /// <para />
                /// NOTE: MD5 is generally considered unsafe.
                /// </summary>
                MD5,

                /// <summary>
                /// Uses Secure Hash Algorithm 1 to get file hashes, used for verifying files in a disk or to see if the file is changed or not before attempting a backup to save time.
                /// </summary>
                SHA1,

                /// <summary>
                /// Uses Secure Hash Algorithm 256 to get file hashes, used for verifying files in a disk or to see if the file is changed or not before attempting a backup to save time.
                /// </summary>
                SHA256,

                /// <summary>
                /// Uses Secure Hash Algorithm 384 to get file hashes, used for verifying files in a disk or to see if the file is changed or not before attempting a backup to save time.
                /// </summary>
                SHA384,

                /// <summary>
                /// Uses Secure Hash Algorithm 512 to get file hashes, used for verifying files in a disk or to see if the file is changed or not before attempting a backup to save time.
                /// </summary>
                SHA512
            }

            public EncryptionAlgorithm Algorithm { get; set; }
            public byte[] PublicKey { get; set; }
            public byte[] PrivateKey { get; set; }
            public byte[] IV { get; set; }

            /// <summary>
            /// Determines which algoritm to use on encrypting and decrypting files while backing up.
            /// </summary>
            public enum EncryptionAlgorithm
            {
                /// <summary>
                /// Don't encrypt the files.
                /// </summary>
                None,

                /// <summary>
                /// Use Advanced Encryption Standard algorithm to encrypt files. Symmetrical, requires <see cref="PublicKey"/> for both ways (encryption and decryption).
                /// </summary>
                AES,

                /// <summary>
                /// uses Triple Data Encryption Standard algorithm to encrypt files. Symmetrical, requires <see cref="PublicKey"/> for both ways (encryption and decryption).
                /// </summary>
                TripleDES,

                /// <summary>
                /// Uses Elliptic Curve Digital Signature Algorithm to encrypt files. Asymmetrical, requires <see cref="PublicKey"/> for encryption and <seealso cref="PrivateKey"/> for decryption (private key).
                /// </summary>
                ECDsa,

                /// <summary>
                /// Uses Rivest–Shamir–Adleman algorithm to encrypt files. Asymmetrical, requires <see cref="PublicKey"/> for encryption and <seealso cref="PrivateKey"/> for decryption.
                /// </summary>
                RSA
            }
        }

        // TODO: Installer, Uploader, Downloader, CopyProgramSelfContained etc. actions.
    }

    public class BackupDirectoryInfo
    {
        public string FileTypes { get; set; }
        public string Name => Path.GetDirectoryName(FullName);
        public string FullName { get; set; }
        public bool IncludeSubDirs { get; set; }
        public string[] Container => Directory.GetFiles(FullName, string.IsNullOrWhiteSpace(FileTypes) ? "*" : FileTypes, IncludeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }
}