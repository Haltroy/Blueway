using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LibFoster;
using static LibFoster.Fostrian;

namespace Blueway.BackupActions
{
    public class BackupDirectoryActionType : BackupActionType
    {
        public override string Name => "Files";

        public override FostrianNode ExportAction(BackupAction action)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        public override BackupAction GenerateAction() => new BackupDirectoryAction();

        public override BackupActionProperty[] GetProperties()
        {
            BackupActionProperty[] bap = new BackupActionProperty[10];
            // TODO
            return bap;
        }

        public override BackupAction ImportAction(FostrianNode node)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }

    #region Enums

    /// <summary>
    /// Determines which algorithm to use on encrypting and decrypting files while backing up.
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

    public enum HashAlgorithm
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

    #endregion Enums

    /// <summary>
    /// Backups a directory with all the files.
    /// </summary>
    public class BackupDirectoryAction : BackupAction
    {
        public override string Name => "Dir";

        public override bool WaitForPreviousActions => true;

        private FileHashInfo[] files;

        private string GetHashFile(string target) => Path.Combine(target, "hashes.fostrian");

        public override void Finalize(string target)
        {
            if (files is null) { return; }
            var root = Fostrian.GenerateRootNode((object)"BluewayHashes");
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var node = root.Add(Path.GetFileName(file.FileName));
                node.Name = "File";
                var hash = node.Add(System.Convert.ToBase64String(file.Hash));
                hash.Name = HashAlgorithm.ToString().ToLowerInvariant();
            }
            Fostrian.Recreate(root, GetHashFile(target));
        }

        public override MultiheadDetails Before(string target, int threadCount)
        {
            List<FileHashInfo> _files = new List<FileHashInfo>();
            if (Args is BackupDirectoryInfo[] args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    _files.AddRange(args[i].GetFiles());
                }
            }
            // TODO: load hashes here
            files = _files.ToArray();
            return new MultiheadDetails(threadCount, files.Length / threadCount, files.Length);
        }

        public override void Run(string target, bool reverse)
        {
            Run(target, reverse, 0, -1);
        }

        public override void Run(string target, bool reverse, int startPoint, int endPoint)
        {
            if (files is null) { return; }
            if (endPoint < 0) { endPoint = files.Length; }
            for (int i = startPoint; i < endPoint; i++)
            {
                if (files[i] is null) { continue; }
                bool skipFile = false;

                switch (HashAlgorithm)
                {
                    default:
                    case HashAlgorithm.None:
                        skipFile = true;
                        files[i].Hash = new byte[0];
                        break;

                    case HashAlgorithm.MD5:
                        var md5hash = HTAlt.Tools.GetHash(files[i].FileName, MD5.Create());
                        if (md5hash == files[i].Hash)
                        {
                            skipFile = true;
                            files[i].Hash = md5hash;
                        }
                        break;

                    case HashAlgorithm.SHA1:
                        var sha1hash = HTAlt.Tools.GetHash(files[i].FileName, SHA1.Create());
                        if (sha1hash == files[i].Hash)
                        {
                            skipFile = true;
                            files[i].Hash = sha1hash;
                        }
                        break;

                    case HashAlgorithm.SHA256:
                        var sha256hash = HTAlt.Tools.GetHash(files[i].FileName, SHA256.Create());
                        if (sha256hash == files[i].Hash)
                        {
                            skipFile = true;
                            files[i].Hash = sha256hash;
                        }
                        break;

                    case HashAlgorithm.SHA384:
                        var sha384hash = HTAlt.Tools.GetHash(files[i].FileName, SHA384.Create());
                        if (sha384hash == files[i].Hash)
                        {
                            skipFile = true;
                            files[i].Hash = sha384hash;
                        }
                        break;

                    case HashAlgorithm.SHA512:
                        var sha512hash = HTAlt.Tools.GetHash(files[i].FileName, SHA512.Create());
                        if (sha512hash == files[i].Hash)
                        {
                            skipFile = true;
                            files[i].Hash = sha512hash;
                        }
                        break;
                }

                if (!skipFile)
                {
                    CopyFile(
                        files[i].FileName,
                        Path.Combine(target, Path.GetFileName(files[i].FileName)),
                        reverse
                    );
                    Progress.Current++;
                }
            }
        }

        private void CopyFile(string file, string targetFile, bool reverse)
        {
            switch (EncryptionAlgorithm)
            {
                case EncryptionAlgorithm.None:
                    File.Copy(file, targetFile, true);
                    break;

                case EncryptionAlgorithm.AES:
                    if (reverse)
                    {
                        //TODO
                    }
                    else
                    {
                        using (
                            FileStream targetStream =
                                new FileStream(
                                    targetFile,
                                    FileMode.OpenOrCreate,
                                    FileAccess.ReadWrite,
                                    FileShare.ReadWrite
                                )
                        )
                        {
                            using (Aes myAES = Aes.Create())
                            {
                                // TODO
                            }
                        }
                    }
                    break;

                case EncryptionAlgorithm.TripleDES:
                    break;

                case EncryptionAlgorithm.ECDsa:
                    break;

                case EncryptionAlgorithm.RSA:
                    break;
            }
        }

        public class FileHashInfo
        {
            public FileHashInfo(string fileName) : this(fileName, new byte[] { })
            {
            }

            public FileHashInfo(string fileName, byte[] hash)
            {
                FileName = string.IsNullOrWhiteSpace(fileName)
                    ? throw new System.ArgumentNullException(nameof(fileName))
                    : fileName;
                Hash = hash ?? throw new System.ArgumentNullException(nameof(hash));
            }

            public string FileName { get; set; }
            public byte[] Hash { get; set; }
        }

        /// <summary>
        /// Algorithms to use when getting hashes for files for later control. These hashes will be noted to a file in the root directory of the backup target directory.
        /// </summary>
        public HashAlgorithm HashAlgorithm { get; set; }

        /// <summary>
        /// Salts the hashes so their outputs will be different than regular hashing. Will generate a salt file with the <see cref="HashSalt"/> data.
        /// </summary>
        public bool HashUseSalt { get; set; }

        /// <summary>
        /// Salts to use for hashing if <see cref="HashUseSalt"/> is enabled.
        /// </summary>
        public byte[] HashSalt { get; set; }

        public EncryptionAlgorithm EncryptionAlgorithm { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] IV { get; set; }

        public class BackupDirectoryInfo
        {
            public BackupDirectoryInfo(string fileTypes, string fullName, bool ıncludeSubDirs)
            {
                FileTypes = fileTypes;
                FullName = fullName;
                IncludeSubDirs = ıncludeSubDirs;
            }

            public string FileTypes { get; set; }
            public string Name => Path.GetDirectoryName(FullName);
            public string FullName { get; set; }
            public bool IncludeSubDirs { get; set; }

            public FileHashInfo[] GetFiles()
            {
                var files = Directory.GetFiles(
                    FullName,
                    string.IsNullOrWhiteSpace(FileTypes) ? "*" : FileTypes,
                    IncludeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );
                var _files = new List<FileHashInfo>();

                for (int i = 0; i < files.Length; i++)
                {
                    _files.Add(new FileHashInfo(files[i]));
                }
                return _files.ToArray();
            }
        }
    }
}