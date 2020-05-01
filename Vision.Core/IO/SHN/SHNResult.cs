using System;
using System.Data;
using System.Globalization;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SHNResult : DataTable
    {
        public int Count { get; protected set; }

        public T Read<T>(int row, string columnName, int number = 0) where T : IConvertible
        {
            return ToOrDefault<T>(Rows[row], columnName + (number != 0 ? (1 + number).ToString() : ""));
        }

        public bool HasValues => Count > 0;

        public byte[] GetBytes(int row, string columnName)
        {
            return (byte[])Rows[row][columnName];
        }

        public object[] ReadAllValuesFromField(string columnName)
        {
            var obj = new object[Count];

            for (var i = 0; i < Count; i++)
                obj[i] = Rows[i][columnName];

            return obj;
        }

        public void Load(SHNFile file)
        {
            using (var data = file.Table.CreateDataReader())
            {
                Load(data);
            }

            Count = file.Table.Rows.Count;
        }

        public T ToOrDefault<T>(DataRow row, string columnName) where T : IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(row[columnName], typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException ex)
            {
                EngineLog.Error($"SHNResult: Invalid Cast (ColumnName {columnName} : {row[columnName]})", ex);
            }
            catch (FormatException ex)
            {
                EngineLog.Error($"SHNResult: Invalid Format (ColumnName {columnName} : {row[columnName]})", ex);
            }
            catch (OverflowException ex)
            {
                EngineLog.Error($"SHNResult: Overflowed (ColumnName {columnName} : {row[columnName]})", ex);
            }
            catch (ArgumentException ex)
            {
                EngineLog.Error($"SHNResult: Invalid Argument (ColumnName {columnName} : {row[columnName]})", ex);
            }
            return default;
        }
	}
}
