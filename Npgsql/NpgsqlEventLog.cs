// created on 07/06/2002 at 09:34

// Npgsql.NpgsqlEventLog.cs
//
// Author:
//    Dave Page (dpage@postgresql.org)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Npgsql
{
    /// <summary>
    /// The level of verbosity of the NpgsqlEventLog
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Don't log at all
        /// </summary>
        None = 0,
        /// <summary>
        /// Only log the most common issues
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Log everything
        /// </summary>
        Debug = 2
    }

    /// <summary>
    /// This class handles all the Npgsql event and debug logging
    /// </summary>
    public class NpgsqlEventLog
    {
        // Logging related values
        public static LogLevel Level { get; set; }
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static String logfile;
        private static Boolean echomessages;

        private NpgsqlEventLog()
        {
        }

        ///<summary>
        /// Sets/Returns the filename to use for logging.
        /// </summary>
        /// <value>The filename of the current Log file.</value>
        public static String LogName
        {
            get
            {
                LogPropertyGet(LogLevel.Debug, CLASSNAME, "LogName");
                return logfile;
            }
            set
            {
                LogPropertySet(LogLevel.Normal, CLASSNAME, "LogName", value);
                logfile = value;
            }
        }

        ///<summary>
        /// Sets/Returns whether Log messages should be echoed to the console
        /// </summary>
        /// <value><b>true</b> if Log messages are echoed to the console, otherwise <b>false</b></value>
        public static Boolean EchoMessages
        {
            get
            {
                LogPropertyGet(LogLevel.Debug, CLASSNAME, "EchoMessages");
                return echomessages;
            }
            set
            {
                LogPropertySet(LogLevel.Normal, CLASSNAME, "EchoMessages", value);
                echomessages = value;
            }
        }

        // Event/Debug Logging
        // This method should be private and only used by the internal methods that support localization.
        /// <summary>
        /// Writes a string to the Npgsql event log if msglevel is bigger then <see cref="Npgsql.NpgsqlEventLog.Level">NpgsqlEventLog.Level</see>
        /// </summary>
        /// <remarks>
        /// This method is obsolete and should no longer be used.
        /// It is likely to be removed in future versions of Npgsql
        /// </remarks>
        /// <param name="message">The message to write to the event log</param>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        private static void LogMsg(String message, LogLevel msglevel)
        {
            if (msglevel > Level)
            {
                return;
            }

            Process proc = Process.GetCurrentProcess();

            if (echomessages)
            {
                Console.WriteLine(message);
            }

            if (!string.IsNullOrEmpty(logfile))
            {
                lock (logfile)
                {
                    StreamWriter writer = new StreamWriter(logfile, true);

                    // The format of the logfile is
                    // [Date] [Time]  [PID]  [Level]  [Message]
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}", DateTime.Now, proc.Id, msglevel, message);
                    writer.Close();
                }
            }

        }

        /// <summary>
        /// Writes a string to the Npgsql event log if msglevel is bigger then <see cref="Npgsql.NpgsqlEventLog.Level">NpgsqlEventLog.Level</see>
        /// </summary>
        /// <param name="resman">The <see cref="System.Resources.ResourceManager">ResourceManager</see> to get the localized resources</param>
        /// <param name="ResourceString">The name of the resource that should be fetched by the <see cref="System.Resources.ResourceManager">ResourceManager</see></param>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="Parameters">The additional parameters that shall be included into the log-message (must be compatible with the string in the resource):</param>
        internal static void LogMsg(string ResourceString, LogLevel msglevel,
                                    params Object[] Parameters)
        {
            if (msglevel > Level)
            {
                return;
            }

            string message = ResourceString;
            if (Parameters.Length > 0)
            {
                message = String.Format(message, Parameters);
            }

            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling the Get-part of an Indexer to the log file.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Indexer</param>
        /// <param name="IndexerParam">The parameter given to the Indexer</param>
        internal static void LogIndexerGet(LogLevel msglevel, string ClassName, object IndexerParam)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Indexer_Get";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling the Set-part of an Indexer to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Indexer</param>
        /// <param name="IndexerParam">The parameter given to the Indexer</param>
        /// <param name="value">The value the Indexer is set to</param>
        internal static void LogIndexerSet(LogLevel msglevel, string ClassName, object IndexerParam, object value)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Indexer_Set";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling the Get-part of a Property to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Property</param>
        /// <param name="PropertyName">The name of the Property</param>
        internal static void LogPropertyGet(LogLevel msglevel, string ClassName, string PropertyName)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Property_Get";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling the Set-part of a Property to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Property</param>
        /// <param name="PropertyName">The name of the Property</param>
        /// <param name="value">The value the Property is set to</param>
        internal static void LogPropertySet(LogLevel msglevel, string ClassName, string PropertyName, object value)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Property_Set";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling a Method without Arguments to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Method</param>
        /// <param name="MethodName">The name of the Method</param>
        internal static void LogMethodEnter(LogLevel msglevel, string ClassName, string MethodName)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Method_0P_Enter";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling a Method with one Argument to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Method</param>
        /// <param name="MethodName">The name of the Method</param>
        /// <param name="MethodParameter">The value of the Argument of the Method</param>
        internal static void LogMethodEnter(LogLevel msglevel, string ClassName, string MethodName, object MethodParameter)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Method_1P_Enter";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling a Method with two Arguments to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Method</param>
        /// <param name="MethodName">The name of the Method</param>
        /// <param name="MethodParameter1">The value of the first Argument of the Method</param>
        /// <param name="MethodParameter2">The value of the second Argument of the Method</param>
        internal static void LogMethodEnter(LogLevel msglevel, string ClassName, string MethodName, object MethodParameter1,
                                            object MethodParameter2)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message ="Method_2P_Enter";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling a Method with three Arguments to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Method</param>
        /// <param name="MethodName">The name of the Method</param>
        /// <param name="MethodParameter1">The value of the first Argument of the Method</param>
        /// <param name="MethodParameter2">The value of the second Argument of the Method</param>
        /// <param name="MethodParameter3">The value of the third Argument of the Method</param>
        internal static void LogMethodEnter(LogLevel msglevel, string ClassName, string MethodName, object MethodParameter1,
                                            object MethodParameter2, object MethodParameter3)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Method_3P_Enter";
            LogMsg(message, msglevel);
        }

        /// <summary>
        /// Writes the default log-message for the action of calling a Method with more than three Arguments to the logfile.
        /// </summary>
        /// <param name="msglevel">The minimum <see cref="Npgsql.LogLevel">LogLevel</see> for which this message should be logged.</param>
        /// <param name="ClassName">The name of the class that contains the Method</param>
        /// <param name="MethodName">The name of the Method</param>
        /// <param name="MethodParameters">A <see cref="System.Object">Object</see>-Array with zero or more Ojects that are Arguments of the Method.</param>
        internal static void LogMethodEnter(LogLevel msglevel, string ClassName, string MethodName,
                                            params object[] MethodParameters)
        {
            if (msglevel > Level)
            {
                return;
            }
            string message = "Method_NP_Enter";
            LogMsg(message, msglevel);
        }
    }
}
