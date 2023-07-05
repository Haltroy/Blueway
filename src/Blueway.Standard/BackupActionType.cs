using LibFoster;
using System;
using System.Collections.Generic;

namespace Blueway
{
    public abstract class BackupActionType
    {
        /// <summary>
        /// Name of the action type. Used for getting translation or finding this specific type on history items.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Registers this action type. Automated by Blueway via C# Reflection.
        /// <para />
        /// DO NOT OVERRIDE!
        /// </summary>
        /// <param name="settings">Instance to load this action type to.</param>
        public void Register(Settings settings)
        {
            settings.RegisterBackupActionType(this);
        }

        /// <summary>
        /// Generates a new action. Create an empty action inherited by base action class (<c>public class MyAction : BackupAction</c>)here.
        /// </summary>
        /// <returns>A new instance of specific action you created that inherits the backup action class.</returns>
        public abstract BackupAction GenerateAction();

        /// <summary>
        /// Gets an array of properties that the user can play with.
        /// </summary>
        /// <returns>An array of properties to be used by Blueway to edit an action.</returns>
        public abstract BackupActionProperty[] GetProperties();

        /// <summary>
        /// Export an action to a simple Fostrian node here.
        /// <para />
        /// Remember to not include properties that are on default value to save space.
        /// </summary>
        /// <param name="action">Action to read. This will be always your custom action class.</param>
        /// <returns>A Fostrian node that has enough information to recreate the action.</returns>
        public abstract void ExportAction(Fostrian.FostrianNode node, BackupAction action);

        /// <summary>
        /// Imports an action from a Fostrian node.
        /// </summary>
        /// <param name="node">The Fostrian node that holds information about your custom action class.</param>
        /// <returns>Your custom action class with all the properties.</returns>
        public abstract BackupAction ImportAction(Fostrian.FostrianNode node);
    }

    public class BackupActionProperty
    {
        public BackupActionProperty(string name, string description, BackupActionPropertyValueType valueType, OnChangeDelegate on_change, GetValueDelegate getValue, object defaultValue = null, string[] options = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            OnChange += on_change;
            GetValue += getValue;
            Options = options ?? new string[0];
            DefaultValue = defaultValue;
            ValueType = valueType;
        }

        public bool MultiPick { get; set; }
        public string DialogTitle { get; set; }
        public string Filters { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public int ByteSize { get; set; }
        public decimal Increment { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public BackupActionPropertyValueType ValueType { get; set; }
        public string[] Options { get; set; }

        public object PerformGetValue(BackupAction action) => GetValue is null ? DefaultValue : GetValue(action);

        public delegate object GetValueDelegate(BackupAction action);

        public event GetValueDelegate GetValue;

        public void PerformChange(BackupAction backupAction, object newVal) => OnChange(backupAction, newVal);

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
        Date,
        Time,
        Options,
        OpenFile,
        SaveFile,
        Folder,
    }
}