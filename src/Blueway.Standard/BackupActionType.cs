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
        public abstract Fostrian.FostrianNode ExportAction(BackupAction action);

        /// <summary>
        /// Imports an action from a Fostrian node.
        /// </summary>
        /// <param name="node">The Fostrian node that holds information about your custom action class.</param>
        /// <returns>Your custom action class with all the properties.</returns>
        public abstract BackupAction ImportAction(Fostrian.FostrianNode node);
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