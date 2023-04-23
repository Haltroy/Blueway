using System;
using System.Collections.Generic;

namespace Blueway
{
    public abstract class BackupActionType
    {
        public void Register(Settings settings)
        {
            // OPTIMIZE
            List<BackupActionType> list = new List<BackupActionType>(settings.BackupActionTypes);
            list.Add(this);
            settings.BackupActionTypes = list.ToArray();
        }

        public abstract BackupAction GenerateAction();

        public abstract BackupActionProperty[] GetProperties();
    }
    public class BackupActionProperty
    {
        public BackupActionProperty(string name, string description, BackupActionPropertyValueType valueType, OnChangeDelegate on_change, object defaultValue = null, string[] options = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            OnChange += on_change;
            Options = options ?? new string[0];
            DefaultValue = defaultValue;
            ValueType = valueType;
        }

        public string Name { get; }
        public string Description { get; }
        public object DefaultValue { get; }
        public BackupActionPropertyValueType ValueType { get; }
        public string[] Options { get; }

        public delegate void OnChangeDelegate(BackupAction backupAction, object newVal);

        public event OnChangeDelegate OnChange;
    }

    public enum BackupActionPropertyValueType
    {
        Boolean,
        Text,
        Password,
        RandomBytes,
        Number,
        SignedNumber,
        Date,
        Time,
        Options,
        File,
        Folder,
        Enumerator,
    }
}