using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Vision.Core.Extensions;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.IO.SHN
{
    public class SimpleSHNFile : IDisposable
    {
		private static readonly EngineLog Logger = new(typeof(SimpleSHNFile));

        private readonly string _filename;
        private readonly ISHNCrypto _crypto;

        protected internal readonly DataTable _table;

        protected internal SimpleSHNFile(string path, ISHNCrypto crypto)
        {
            _filename = path;
            _crypto = crypto;
            _table = new DataTable();
            PopulateTable();
		}

		private static Type GetColumnDataType(uint type)
        {
            switch (type)
            {
                case 1:
                case 12:
                case 16:
                    return typeof(byte);
                case 2:
                    return typeof(ushort);
                case 3:
                case 11:
                case 18:
                case 27:
                    return typeof(uint);
                case 5:
                    return typeof(float);
                case 13:
                case 21:
                    return typeof(short);
                case 20:
                    return typeof(sbyte);
                case 22:
                    return typeof(int);
                case 9:
                case 24:
                case 26:
                    return typeof(string);
                default:
                    return typeof(object);
            }
        }

		private void PopulateTable()
		{
			if (!File.Exists(_filename))
			{
                Logger.Error($"The file '{_filename}' could not be found.");
				return;
			}

			byte[] buffer;

			using (var file = File.OpenRead(_filename))
			using (var reader = new BinaryReader(file))
			{
				reader.ReadBytes(32); // Crypt header, unused.

				var length = reader.ReadInt32();
				buffer = reader.ReadBytes(length - 36);

				_crypto.Decrypt(buffer);
			}

			using (var stream = new MemoryStream(buffer))
			using (var reader = new BinaryReader(stream))
			{
				reader.ReadBytes(4); // Header, unused.

				var rowCount = reader.ReadUInt32();
				var defaultRowLength = reader.ReadUInt32(); // Default row length, unused.
				var columnCount = reader.ReadUInt32();

				var columnTypes = new uint[columnCount];
				var columnLengths = new int[columnCount];

				var unkColumnCount = 0;

				for (var i = 0; i < columnCount; i++)
				{
					var name = reader.ReadString(48);
					var type = reader.ReadUInt32();
					var length = reader.ReadInt32();

					var column = new DataColumn(name, GetColumnDataType(type));

					if (name.Trim().Length < 2)
					{
						column.ColumnName = $"UnkCol{unkColumnCount++}";
					}

					columnTypes[i] = type;
					columnLengths[i] = length;

					_table.Columns.Add(column);
				}

				var row = new object[columnCount];

				for (var i = 0; i < rowCount; i++)
				{
					var rowLength = reader.ReadUInt16();

					for (var j = 0; j < columnCount; j++)
					{
						switch (columnTypes[j])
						{
							case 1:
							case 12:
							case 16:
								row[j] = reader.ReadByte();
								break;
							case 2:
								row[j] = reader.ReadUInt16();
								break;
							case 3:
							case 11:
							case 18:
							case 27:
								row[j] = reader.ReadUInt32();
								break;
							case 5:
								row[j] = reader.ReadSingle();
								break;
							case 9:
							case 24:
								row[j] = reader.ReadString(columnLengths[j]);
								break;
							case 13:
							case 21:
								row[j] = reader.ReadInt16();
								break;
							case 20:
								row[j] = reader.ReadSByte();
								break;
							case 22:
								row[j] = reader.ReadInt32();
								break;
							case 26:
								row[j] = reader.ReadString(rowLength - (int)defaultRowLength + 1);
								break;
							case 29:
								var bytes = reader.ReadBytes(columnLengths[j]);

								var val1 = BitConverter.ToUInt32(bytes, 0);
								var val2 = BitConverter.ToUInt32(bytes, 4);

								row[j] = new Tuple<uint, uint>(val1, val2);
								break;
						}
					}

					_table.Rows.Add(row);
				}
			}
		}

		public void Dispose() => _table.Dispose();
    }
}
