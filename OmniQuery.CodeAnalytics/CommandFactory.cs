using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using System.Data.SQLite;

namespace OmniQuery.CodeAnalytics
{
    internal class CommandFactory : IDisposable
    {
        private bool disposed;
        SQLiteConnection _conn;
        Dictionary<string, DbCommand> _insertCommands = new Dictionary<string, DbCommand>();
        Dictionary<string, DbCommand> _updateCommands = new Dictionary<string, DbCommand>();
        Dictionary<string, long> _insertResults = new Dictionary<string, long>();

        public CommandFactory(SQLiteConnection conn)
        {
            _conn = conn;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (DbCommand cmd in _insertCommands.Values)
                    {
                        cmd.Dispose();
                    }

                    foreach (DbCommand cmd in _updateCommands.Values)
                    {
                        cmd.Dispose();
                    }

                    _insertCommands = null;
                    _updateCommands = null;
                    _insertResults = null;
                    _conn = null;
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Insert a record of given enum type into the database
        /// </summary>
        /// <typeparam name="T">Enumeration type of record to create</typeparam>
        /// <param name="paramValues">Field values</param>
        public void Insert<T>(params object[] paramValues) where T : struct
        {
            Type enumType = typeof(T);
            DbCommand cmd;

            if (_insertCommands.ContainsKey(enumType.Name))
            {
                cmd = _insertCommands[enumType.Name];
            }
            else
            {
                string[] fields = Enum.GetNames(enumType);
                string placeholders = string.Join("?,", new string[fields.Length + 1]).TrimEnd(',');

                cmd = _conn.CreateCommand();
                cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT last_insert_rowid();",
                    enumType.Name.Replace("Field", ""), string.Join(",", fields), placeholders);

                for (int i = 0; i < fields.Length; i++)
                {
                    cmd.Parameters.Add(cmd.CreateParameter());
                }

                _insertCommands[enumType.Name] = cmd;
            }

            for (int i = 0; i < paramValues.Length; i++)
            {
                cmd.Parameters[i].Value = FormatValue(paramValues[i]);
            }

            _insertResults[enumType.Name] = (long)cmd.ExecuteScalar();
        }

        /// <summary>
        /// Return the ID of the last inserted record for a given enum type
        /// </summary>
        /// <typeparam name="T">Enumeration type of record created</typeparam>
        /// <returns>ID of last inserted record</returns>
        public long GetLastId<T>() where T : struct
        {
            return _insertResults[typeof(T).Name];
        }

        /// <summary>
        /// Update a record of given enum type
        /// </summary>                                         
        /// <typeparam name="T">Enumeration type of record to update</typeparam>
        /// <param name="field">Enumeration value of the field to update</param>
        /// <param name="value">New value</param>
        public void Update<T>(T field, object value) where T : struct
        {
            Type enumType = typeof(T);
            string fullName = enumType.Name + "." + field;
            DbCommand cmd;

            if (_updateCommands.ContainsKey(fullName))
            {
                cmd = _updateCommands[fullName];
            }
            else
            {
                cmd = _conn.CreateCommand();
                cmd.CommandText = string.Format("UPDATE {0} SET {1} = ? WHERE Id = ?",
                    enumType.Name.Replace("Field", ""), field);

                cmd.Parameters.Add(cmd.CreateParameter());
                cmd.Parameters.Add(cmd.CreateParameter());
                _updateCommands[fullName] = cmd;
            }

            cmd.Parameters[0].Value = FormatValue(value);
            cmd.Parameters[1].Value = GetLastId<T>();
            cmd.ExecuteScalar();
        }

        /// <summary>
        /// Format value for saving into database
        /// </summary>
        /// <param name="value">Value to be formatted</param>
        /// <returns>Formatted value</returns>
        private object FormatValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            string strValue = value as string;
            if (strValue != null)
            {
                return string.IsNullOrEmpty(strValue.Trim()) ? null : strValue;
            }

            Enum enumValue = value as Enum;
            if (enumValue != null)
            {
                return enumValue.ToString();
            }

            byte[] byteValue = value as byte[];
            if (byteValue != null)
            {
                if (byteValue.Length == 0)
                {
                    return null;
                }
                else
                {
                    foreach (byte val in byteValue)
                    {
                        strValue += val.ToString("x2");
                    }
                    return strValue;
                }
            }

            return value;
        }
    }
}
