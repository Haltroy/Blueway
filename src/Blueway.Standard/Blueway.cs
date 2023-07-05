// Ignore Spelling: Multihead

using System;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Blueway
{
    public class BackupHistoryItem
    {
        public BackupHistoryItem Self => this;

        public BackupSchema Schema { get; set; }

        public DateTime Date { get; set; }

        public BackupStatus Status { get; set; }
        public bool IsSuccess => Status == BackupStatus.Success;
        public bool IsOngoing => Status == BackupStatus.OnGoing;
        public bool IsPlanned => Status == BackupStatus.Planned;
        public bool IsFailure => Status == BackupStatus.Failure;
        public string Name => Schema.Name;

        public ScheduleInfo Schedule => Schema.Schedule;

        public string BackupDir { get; set; }
    }

    public class ScheduleInfo : IEquatable<ScheduleInfo>
    {
        private int milliseconds = 0;
        private int seconds = 0;
        private int minutes = 0;
        private int hours = 0;
        private int days = 0;
        private int months = 0;

        public ScheduleInfo()
        { }

        public ScheduleInfo(int milliseconds, int seconds, int minutes, int hours, int days, int months, int years)
        {
            Milliseconds = milliseconds;
            Seconds = seconds;
            Minutes = minutes;
            Hours = hours;
            Days = days;
            Months = months;
            Years = years;
        }

        public static ScheduleInfo Empty => new ScheduleInfo(0, 0, 0, 0, 0, 0, 0);

        public override string ToString() => $"{(Hours < 10 ? "0" : "") + Hours}:{(Minutes < 10 ? "0" : "") + Minutes}:{(Seconds < 10 ? "0" : "") + Seconds}:{(Milliseconds < 10 ? "0" : "") + (Milliseconds < 100 ? "0" : "") + Milliseconds} {(Days < 10 ? "0" : "") + Days}/{(Months < 10 ? "0" : "") + Months}/{(Years < 10 ? "0" : "") + Years}";

        public long ToBinary() => milliseconds +
                (seconds * 1000) +
                (minutes * 60000) +
                (hours * 3600000) +
                (days * 86400000) +
                (months * 2592000000) +
                (Years * 31104000000);

        public static ScheduleInfo FromBinary(long l)
        {
            int y = (int)(l / 31104000000);
            l %= 31104000000;

            int mm = (int)(l / 2592000000);
            l %= 2592000000;

            int d = (int)(l / 86400000);
            l %= 86400000;

            int h = (int)(l / 3600000);
            l %= 3600000;

            int m = (int)(l / 60000);
            l %= 60000;

            int s = (int)(l / 1000);
            int ms = (int)(l % 1000);

            return new ScheduleInfo()
            {
                Milliseconds = ms,
                Seconds = s,
                Minutes = m,
                Hours = h,
                Days = d,
                Months = mm,
                Years = y
            };
        }

        public static ScheduleInfo FromMilliseconds(long ms) => FromBinary(ms);

        public static ScheduleInfo FromSeconds(long s) => FromBinary(s * 1000);

        public static ScheduleInfo FromMinutes(long m) => FromBinary(m * 60000);

        public static ScheduleInfo FromHours(long h) => FromBinary(h * 3600000);

        public static ScheduleInfo FromDays(long d) => FromBinary(d * 86400000);

        public static ScheduleInfo FromMonths(long m) => FromBinary(m * 2592000000);

        public static ScheduleInfo FromYears(int y) => new ScheduleInfo(0, 0, 0, 0, 0, 0, y);

        public static bool TryParse(string s, out ScheduleInfo result)
        {
            try
            {
                result = Parse(s);
                return true;
            }
            catch
            {
                result = Empty;
                return false;
            }
        }

        /// <summary>
        /// Parses <paramref name="s"/>.
        /// <para />
        /// Format: hours:minutes:seconds:milliseconds day/month/year
        /// <para />
        /// Note: Only accepts numbers!
        /// </summary>
        /// <param name="s">hours:minutes:seconds:milliseconds day/month/year</param>
        /// <returns><see cref="ScheduleInfo"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="s"/> is null, empty or consists of white space.</exception>
        /// <exception cref="ArgumentException">Thrown for various parsing errors.</exception>
        public static ScheduleInfo Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }
            if (!s.Contains(" "))
            {
                throw new ArgumentException("No space between time and date.", nameof(s));
            }

            if (!s.Contains(":"))
            {
                throw new ArgumentException("No time divider (:).", nameof(s));
            }
            if (!s.Contains("/") && !s.Contains("\\"))
            {
                throw new ArgumentException("No date divider (\"\\\" or \"/\").", nameof(s));
            }
            ScheduleInfo info = new ScheduleInfo();
            string date = s.Split(' ')[1];
            string time = s.Split(' ')[0];

            string[] time_info = time.Split(':');
            for (int i = 0; i < time_info.Length; i++)
            {
                if (int.TryParse(time_info[i], out int _t))
                {
                    switch (i)
                    {
                        case 0:
                            info.Hours = _t;
                            break;

                        case 1:
                            info.Minutes = _t;
                            break;

                        case 2:
                            info.Seconds = _t;
                            break;

                        case 3:
                            info.Milliseconds = _t;
                            break;

                        default:
                            // Not Implemented.
                            break;
                    }
                }
                else
                {
                    throw new ArgumentException($"Cannot convert the text into an integer at {i} in time section.", nameof(s));
                }
            }
            date = date.Replace("\\", "/");
            string[] date_info = date.Split('/');
            for (int i = 0; i < date_info.Length; i++)
            {
                if (int.TryParse(date_info[i], out int _t))
                {
                    switch (i)
                    {
                        case 0:
                            info.Days = _t;
                            break;

                        case 1:
                            info.Months = _t;
                            break;

                        case 2:
                            info.Years = _t;
                            break;

                        default:
                            // Not Implemented.
                            break;
                    }
                }
                else
                {
                    throw new ArgumentException($"Cannot convert the text into an integer at {i} in date section.", nameof(s));
                }
            }

            return info;
        }

        public DateTime NextOccurrence(DateTime previous) => previous.AddMilliseconds(Milliseconds).AddSeconds(Seconds).AddMinutes(Minutes).AddHours(Hours).AddDays(Days).AddMonths(Months).AddYears(Years);

        public bool IsNextOccurrence(DateTime previous) => DateTime.Equals(NextOccurrence(previous), DateTime.Now);

        public override bool Equals(object obj)
        {
            return Equals(obj as ScheduleInfo);
        }

        public bool Equals(ScheduleInfo other)
        {
            return !(other is null) &&
                   Milliseconds == other.Milliseconds &&
                   Seconds == other.Seconds &&
                   Minutes == other.Minutes &&
                   Hours == other.Hours &&
                   Days == other.Days &&
                   Months == other.Months &&
                   Years == other.Years;
        }

        public override int GetHashCode()
        {
            int hashCode = -1052354240;
            hashCode = hashCode * -1521134295 + Milliseconds.GetHashCode();
            hashCode = hashCode * -1521134295 + Seconds.GetHashCode();
            hashCode = hashCode * -1521134295 + Minutes.GetHashCode();
            hashCode = hashCode * -1521134295 + Hours.GetHashCode();
            hashCode = hashCode * -1521134295 + Days.GetHashCode();
            hashCode = hashCode * -1521134295 + Months.GetHashCode();
            hashCode = hashCode * -1521134295 + Years.GetHashCode();
            return hashCode;
        }

        public int Milliseconds
        {
            get => milliseconds;
            set
            {
                milliseconds = value % 1000;
                Seconds += value / 1000;
            }
        }

        public int Seconds
        {
            get => seconds; set
            {
                seconds = value % 60;
                Minutes += value / 60;
            }
        }

        public int Minutes
        {
            get => minutes; set
            {
                minutes = value % 60;
                Hours += value / 60;
            }
        }

        public int Hours
        {
            get => hours; set
            {
                hours = value % 24;
                Days += value / 24;
            }
        }

        public int Days
        {
            get => days; set
            {
                days = value % 30;
                Months += value / 30;
            }
        }

        public int Months
        {
            get => months; set
            {
                months = value % 12;
                Years += value / 12;
            }
        }

        public bool IsNotEmpty => this != Empty;

        public int Years { get; set; } = 0;

        public static bool operator ==(ScheduleInfo left, ScheduleInfo right)
        {
            return EqualityComparer<ScheduleInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(ScheduleInfo left, ScheduleInfo right)
        {
            return !(left == right);
        }
    }

    public class BackupSchema
    {
        public string Name { get; set; }
        public ScheduleInfo Schedule { get; set; } = ScheduleInfo.Empty;

        public string BackupDir { get; set; }

        public BackupSchema(List<BackupAction> actions)
        {
            Actions = actions;
        }

        public List<BackupAction> Actions { get; set; }

        public delegate void OnEventDelegate(BackupSchema schema);

        public delegate void OnProgressChangeDelegate(BackupSchema schema, Progress progress);

        public event OnEventDelegate OnStart;

        public event OnEventDelegate OnEnd;

        public event OnProgressChangeDelegate OnProgressChange;

        public void Run()
        {
        }
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
        private Progress progress = new Progress();

        /// <summary>
        /// Status of current action.
        /// </summary>
        public BackupStatus Status { get; set; }

        /// <summary>
        /// Arguments of the action.
        /// </summary>
        public object Args { get; set; }

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
        public Progress Progress
        { get => progress; set { OnProgressChange?.Invoke(this, value); progress = value; } }

        /// <summary>
        /// Delegate event for <see cref="OnProgressChange" />.
        /// </summary>
        /// <param name="action">Action that caused the change.</param>
        /// <param name="progress">New value of progress.</param>
        public delegate void OnProgressChangeDelegate(BackupAction action, Progress progress);

        /// <summary>
        /// Event raised on <see cref="Progress"/> changed .
        /// </summary>
        public event OnProgressChangeDelegate OnProgressChange;

        /// <summary>
        /// Delegate event for <see cref="OnDone" />.
        /// </summary>
        /// <param name="action">Action that is done.</param>
        public delegate void OnEventDelegate(BackupAction action);

        /// <summary>
        /// Event raised when this action has done. This event should be raised by the schema itself.
        /// </summary>
        public event OnEventDelegate OnDone;

        /// <summary>
        /// Event raised when this action is started. This event should be raised by the schema itself.
        /// </summary>
        public event OnEventDelegate OnStart;

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

        public BackupActionType GetBackupActionType { get; }
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

        public bool IsIndeterminate { get; set; }
    }
}